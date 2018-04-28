using StudentTask.Model;
using StudentTask.Uwp.App.DataSource;
using System.Collections.Generic;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
using Task = System.Threading.Tasks.Task;

namespace StudentTask.Uwp.App.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        public User SessionUser { get; set; }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (Users.Instance.SessionUser.Courses == null)
                await Courses.Instance.GetUserCourses(Users.Instance.SessionUser);
            if (Users.Instance.SessionUser.Tasks == null)
                await Tasks.Instance.GetTasks(Users.Instance.SessionUser);

            SessionUser = Users.Instance.SessionUser;
        }
    }
}
