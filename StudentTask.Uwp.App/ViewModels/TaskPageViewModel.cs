using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;
using StudentTask.Model;
using Template10.Mvvm;
using Task = System.Threading.Tasks.Task;

namespace StudentTask.Uwp.App.ViewModels
{
    public class TaskPageViewModel : ViewModelBase
    {
        public Student SessionStudent { get; set; }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (SessionStudent == null)
                SessionStudent = DataSource.Students.Instance.UserStudent;

            if(SessionStudent.Tasks == null)
                SessionStudent.Tasks = new List<Model.Task>(await DataSource.Students.Instance.GetTasks(SessionStudent));

            await Task.CompletedTask;
        }
    }
}
