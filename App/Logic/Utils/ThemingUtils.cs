﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using AdonisUI;
using App.Annotations;
using MVVM_Tools.Code.Classes;

namespace App.Logic.Utils
{
    public class ThemingUtils : BindableBase
    {
        [NotNull] private readonly AppSettings _appSettings;

        public AppThemes CurrentTheme
        {
            get => _appSettings.AppTheme;
            set => _appSettings.AppTheme = value;
        }

        public IReadOnlyList<AppThemes> AvailableThemes { get; } = (AppThemes[]) Enum.GetValues(typeof(AppThemes));

        public ThemingUtils([NotNull] AppSettings appSettings)
        {
            _appSettings = appSettings;

            appSettings.PropertyChanged += AppSettings_OnPropertyChanged;
        }

        public void ApplyCurrent()
        {
            Uri themeUri;
            switch (CurrentTheme)
            {
                case AppThemes.Light:
                    themeUri = ResourceLocator.LightColorScheme;
                    break;
                case AppThemes.Dark:
                    themeUri = ResourceLocator.DarkColorScheme;
                    break;
                default:
                    throw new ArgumentException($"Unknown app theme: `{CurrentTheme}`");
            }
            ResourceLocator.SetColorScheme(Application.Current.Resources, themeUri);
        }

        private void AppSettings_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(AppSettings.AppTheme):
                    OnPropertyChanged(nameof(CurrentTheme));
                    ApplyCurrent();
                    break;
            }
        }
    }
}
