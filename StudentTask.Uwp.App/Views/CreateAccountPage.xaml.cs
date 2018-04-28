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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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

            if (await DataSource.Users.Instance.CreateUser(NewUser))
            {
                Frame.Navigate(typeof(LogOnPage));
            }
            else
            {
                // TODO: Display why creating user failed.
            }
        }
    }
}
