using System;
using Windows.UI.Xaml;
using StudentTask.Uwp.App.Views;
using Template10.Common;
using Template10.Services.SettingsService;
using Template10.Utils;

namespace StudentTask.Uwp.App.Services.SettingsServices
{
    public class SettingsService
    {
        private readonly ISettingsHelper _helper;

        private SettingsService()
        {
            _helper = new SettingsHelper();
        }

        public static SettingsService Instance { get; } = new SettingsService();

        public bool UseShellBackButton
        {
            get => _helper.Read(nameof(UseShellBackButton), true);
            set
            {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.GetDispatcherWrapper().Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                });
            }
        }

        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = ApplicationTheme.Light;
                var value = _helper.Read(nameof(AppTheme), theme.ToString());
                return Enum.TryParse(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                ((FrameworkElement) Window.Current.Content).RequestedTheme = value.ToElementTheme();
                Shell.HamburgerMenu.RefreshStyles(value, true);
            }
        }

        public TimeSpan CacheMaxDuration
        {
            get => _helper.Read(nameof(CacheMaxDuration), TimeSpan.FromDays(2));
            set
            {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        public bool ShowHamburgerButton
        {
            get => _helper.Read(nameof(ShowHamburgerButton), true);
            set
            {
                _helper.Write(nameof(ShowHamburgerButton), value);
                Shell.HamburgerMenu.HamburgerButtonVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool IsFullScreen
        {
            get => _helper.Read(nameof(IsFullScreen), false);
            set
            {
                _helper.Write(nameof(IsFullScreen), value);
                Shell.HamburgerMenu.IsFullScreen = value;
            }
        }
    }
}