﻿using Windows.UI.Xaml;
using StudentTask.Uwp.App.Services.SettingsServices;
using Template10.Controls;
using Template10.Services.NavigationService;

namespace StudentTask.Uwp.App.Views
{
    public sealed partial class Shell
    {
        private readonly SettingsService _settings;

        public Shell()
        {
            Instance = this;
            InitializeComponent();
            _settings = SettingsService.Instance;
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
            HamburgerMenu.RefreshStyles(_settings.AppTheme, true);
            HamburgerMenu.IsFullScreen = _settings.IsFullScreen;
            HamburgerMenu.HamburgerButtonVisibility =
                _settings.ShowHamburgerButton ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}