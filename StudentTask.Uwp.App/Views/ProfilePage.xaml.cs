// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238


using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using StudentTask.Model;
using StudentTask.Uwp.App.DataSource;

namespace StudentTask.Uwp.App.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.Page" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public sealed partial class ProfilePage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProfilePage" /> class.
        /// </summary>
        public ProfilePage()
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
            if (Users.Instance.SessionUser.GroupUserGroup == User.UserGroup.Admin)
                AddNewUserButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Adds the new user.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void AddNewUser(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateAccountPage));
        }

        private void LogOutButton_OnClick(object sender, RoutedEventArgs e)
        {
            Users.Instance.SessionUser = null;
            Frame.Navigate(typeof(LogOnPage));
        }
    }
}