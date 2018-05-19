// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238


using Windows.Devices.AllJoyn;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using StudentTask.Model;

namespace StudentTask.Uwp.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilePage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataSource.Users.Instance.SessionUser.GroupUserGroup == User.UserGroup.Admin)
                AddNewUserButton.Visibility = Visibility.Visible;
        }

        private void AddNewUser(object sender, RoutedEventArgs e) => Frame.Navigate(typeof(CreateAccountPage));
    }
}
