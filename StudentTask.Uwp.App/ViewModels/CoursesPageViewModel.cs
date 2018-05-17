using System;
using StudentTask.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
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
                Courses = new ObservableCollection<Course>(
                    await DataSource.Courses.Instance.GetUserCourses(DataSource.Users.Instance.SessionUser));
            }
        }
    }
}
