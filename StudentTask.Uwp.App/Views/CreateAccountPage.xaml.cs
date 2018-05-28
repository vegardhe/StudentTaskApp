using System;
using System.Net;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using StudentTask.Model;
using StudentTask.Uwp.App.DataSource;
using Task = System.Threading.Tasks.Task;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StudentTask.Uwp.App.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.Page" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public sealed partial class CreateAccountPage
    {
        /// <summary>
        ///     The email regex
        /// </summary>
        private static readonly Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        /// <summary>
        ///     Initializes a new instance of the <see cref="CreateAccountPage" /> class.
        /// </summary>
        public CreateAccountPage()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the new user.
        /// </summary>
        /// <value>
        ///     The new user.
        /// </value>
        private User NewUser { get; set; }

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
        ///     Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">
        ///     Event data that can be examined by overriding code. The event data is representative of the pending
        ///     navigation that will load the current Page. Usually the most relevant property to examine is Parameter.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CheckUser(Users.Instance.SessionUser);

            Shell.HamburgerMenu.IsFullScreen = true;
            NewUser = new User();
            NewAccountGrid.DataContext = NewUser;
        }

        private void CheckUser(User user)
        {
            if (user == null || user.GroupUserGroup != User.UserGroup.Admin) return;
            UsergroupTextBlock.Visibility = Visibility.Visible;
            UsergroupComboBox.Visibility = Visibility.Visible;
            UsergroupComboBox.ItemsSource = Enum.GetValues(typeof(User.UserGroup));
        }

        /// <summary>
        ///     Handles the OnClick event of the CreateAccountButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private async void CreateAccountButton_OnClick(object sender, RoutedEventArgs e)
        {
            var match = EmailRegex.Match(NewUser.Email);
            if (!match.Success)
            {
                ErrorTextBlock.Text = "Email is not valid!";
                return;
            }

            if (PasswordBox.Password != RepeatPasswordBox.Password)
            {
                ErrorTextBlock.Text = "Passwords does not match!";
                return;
            }

            if (Users.Instance.SessionUser != null &&
                Users.Instance.SessionUser.GroupUserGroup == User.UserGroup.Admin)
                if (UsergroupComboBox.SelectedValue != null)
                    NewUser.GroupUserGroup = (User.UserGroup) UsergroupComboBox.SelectedValue;
            await AddUser(NewUser);
        }

        /// <summary>
        ///     Adds the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        private async Task AddUser(User user)
        {
            try
            {
                await Users.Instance.PostUser(user);
            }
            catch (WebException ex)
            {
                await ex.Log();
                await ex.Display("Failed to establish internet connection.");
            }
            catch (Exception ex)
            {
                await ex.Log();
                await ex.Display("Failed to create user.");
            }

            Frame.Navigate(Users.Instance.SessionUser != null ? typeof(ProfilePage) : typeof(LogOnPage));
        }
    }
}