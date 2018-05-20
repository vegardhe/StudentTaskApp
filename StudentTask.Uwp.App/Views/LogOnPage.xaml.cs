using StudentTask.Model;
using System;
using System.IO;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StudentTask.Uwp.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogOnPage
    {
        public LogOnPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e) => Shell.HamburgerMenu.IsFullScreen = true;

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) => Shell.HamburgerMenu.IsFullScreen = false;

        private async void Login(object sender, RoutedEventArgs e)
        {
            var user = new User {Username = UsernameBox.Text, Password = PasswordBox.Password};
            ProgressRing.IsActive = true;
            try
            {
                if (await DataSource.Users.Instance.LogOn(user) != null)
                    Frame.Navigate(typeof(TaskPage));
            }
            catch (WebException ex)
            {
                await ex.Display("Could not establish a connection.");
            }
            catch (InvalidDataException ex)
            {
                ErrorBlock.Text = "Invalid username or password.";
                await ex.Log();
            }
            catch (Exception ex)
            {
                await ex.Display("An error occured.");
            }
            finally
            {
                ProgressRing.IsActive = false;
            }
        }

        private void Hyperlink_OnClick(Windows.UI.Xaml.Documents.Hyperlink sender,
            Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args) => Frame.Navigate(typeof(CreateAccountPage));
    }
}
