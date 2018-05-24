using StudentTask.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
using StudentTask.Uwp.App.DataSource;
using StudentTask.Uwp.App.Views;
using Task = System.Threading.Tasks.Task;

namespace StudentTask.Uwp.App.ViewModels
{
    public class CoursesPageViewModel : ViewModelBase
    {
        public ObservableCollection<Course> Courses { get; private set; }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {

            if (Courses == null)
            {
                try
                {
                    Courses = new ObservableCollection<Course>(
                        await DataSource.Courses.Instance.GetUserCourses(Users.Instance.SessionUser));
                }
                catch (HttpRequestException e)
                {
                    await e.Log();
                    await e.Display("Requesting courses timed out.");
                }
            }
        }
    }
}
