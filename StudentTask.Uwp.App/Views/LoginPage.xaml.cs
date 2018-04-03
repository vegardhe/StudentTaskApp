﻿using StudentTask.Model;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StudentTask.Uwp.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Views.Shell.HamburgerMenu.IsFullScreen = true;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Views.Shell.HamburgerMenu.IsFullScreen = false;
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            var student = new Student { Username = UsernameBox.Text, Password = PasswordBox.Password};
            ProgressRing.IsActive = true;
            try
            {
                Student loginStudent;
                if ((loginStudent = await DataSource.Students.Instance.Login(student)) != null)
                    Frame.Navigate(typeof(TaskPage));
                else
                    ErrorBlock.Text = "Invalid username/password.";
            }
            catch (Exception ex)
            {
                //TODO: Exception handling.
            }
            ProgressRing.IsActive = false;
        }
    }
}
