using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;
using StudentTask.Model;
using StudentTask.Uwp.App.DataSource;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StudentTask.Uwp.App.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.Page" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public sealed partial class LogOnPage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogOnPage" /> class.
        /// </summary>
        public LogOnPage()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">
        ///     Event data that can be examined by overriding code. The event data is representative of the pending
        ///     navigation that will load the current Page. Usually the most relevant property to examine is Parameter.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Shell.HamburgerMenu.IsFullScreen = true;
        }

        /// <summary>
        ///     Invoked immediately before the Page is unloaded and is no longer the current source of a parent Frame.
        /// </summary>
        /// <param name="e">
        ///     Event data that can be examined by overriding code. The event data is representative of the navigation
        ///     that will unload the current Page unless canceled. The navigation can potentially be canceled by setting Cancel.
        /// </param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Shell.HamburgerMenu.IsFullScreen = false;
        }

        /// <summary>
        ///     Logins the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private async void Login(object sender, RoutedEventArgs e)
        {
            var user = new User {Username = UsernameBox.Text, Password = PasswordBox.Password};
            ProgressRing.IsActive = true;
            try
            {
                if (await Users.Instance.LogOn(user) != null)
                    Frame.Navigate(typeof(TaskPage));
            }
            catch (WebException ex)
            {
                await ex.Display("Could not establish a connection to internet.");
            }
            catch (CommunicationException ex)
            {
                await ex.Display("Could not establish a database connection.");
                await ex.Log();
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

        /// <summary>
        ///     Hyperlinks the on click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">
        ///     The <see cref="Windows.UI.Xaml.Documents.HyperlinkClickEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private void Hyperlink_OnClick(Hyperlink sender,
            HyperlinkClickEventArgs args)
        {
            Frame.Navigate(typeof(CreateAccountPage));
        }
    }
}