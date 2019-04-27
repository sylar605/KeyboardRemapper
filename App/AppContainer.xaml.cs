﻿using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using App.Logic;
using App.Operations;
using App.ViewModels;
using Autofac;
using SettingsManager;
using SettingsManager.ModelProcessors;

namespace App
{
    public partial class AppContainer
    {
        private IContainer _container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var container = new ContainerBuilder();

            // windows
            container.RegisterType<MainWindow>().SingleInstance();
            container.RegisterType<NewMappingWindow>();

            // view models
            container.RegisterType<MainWindowViewModel>().SingleInstance();
            container.RegisterType<NewMappingWindowViewModel>();

            // operations
            container.RegisterType<MappingOperation>().SingleInstance();

            container.Register(context =>
            {
                return new SettingsBuilder<AppSettings>()
                    .WithFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "appSettings.json"))
                    .WithProcessor(new JsonModelProcessor())
                    .Build();
            }).SingleInstance().As<AppSettings>();
            container.RegisterType<HooksHandler>().SingleInstance();
            container.RegisterGeneric(typeof(Provider<>)).SingleInstance();
            container.RegisterType<NotifyIcon>().SingleInstance();
            container.RegisterType<NotifyIconHolder>().SingleInstance();
            container.RegisterType<KeyMappingsHandler>().SingleInstance();

            _container = container.Build();

            Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _container.Resolve<KeyMappingsHandler>().Dispose();
            _container.Resolve<HooksHandler>().Dispose();
            _container.Resolve<NotifyIconHolder>().Dispose();

            base.OnExit(e);
        }

        private void Initialize()
        {
            // constructor invocation starts hooking
            _container.Resolve<KeyMappingsHandler>();

            // window will be shown minimized
            _container.Resolve<MainWindow>().Show();
            _container.Resolve<NotifyIconHolder>().NotifyIcon.Visible = true;
        }
    }
}
