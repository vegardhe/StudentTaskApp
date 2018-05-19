using System;
using StudentTask.Model;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StudentTask.Uwp.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateAccountPage
    {
        private User NewUser { get; set; }

        private static readonly Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        public CreateAccountPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) => Shell.HamburgerMenu.IsFullScreen = false;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataSource.Users.Instance.SessionUser != null && DataSource.Users.Instance.SessionUser.GroupUserGroup == User.UserGroup.Admin)
            {
                UsergroupTextBlock.Visibility = Visibility.Visible;
                UsergroupComboBox.Visibility = Visibility.Visible;
                UsergroupComboBox.ItemsSource = Enum.GetValues(typeof(User.UserGroup));
            }
            Shell.HamburgerMenu.IsFullScreen = true;
            NewUser = new User();
            NewAccountGrid.DataContext = NewUser;
        }

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

            if (DataSource.Users.Instance.SessionUser != null &&
                DataSource.Users.Instance.SessionUser.GroupUserGroup == User.UserGroup.Admin)
            {
                if (UsergroupComboBox.SelectedValue != null)
                    NewUser.GroupUserGroup = (User.UserGroup) UsergroupComboBox.SelectedValue;
            }

            if (await DataSource.Users.Instance.CreateUser(NewUser))
            {
                Frame.Navigate(DataSource.Users.Instance.SessionUser != null ? typeof(ProfilePage) : typeof(LogOnPage));
            }
            else
            {
                // TODO: Display why creating user failed.
            }
        }
    }
}
