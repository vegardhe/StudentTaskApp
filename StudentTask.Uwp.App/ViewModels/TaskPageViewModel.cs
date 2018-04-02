using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Navigation;
using StudentTask.Model;
using Template10.Mvvm;
using Task = System.Threading.Tasks.Task;

namespace StudentTask.Uwp.App.ViewModels
{
    public class TaskPageViewModel : ViewModelBase
    {
        public Student SessionStudent { get; set; }

        public ObservableCollection<Model.Task> Tasks { get; set; }

        public static List<Model.Task.Status> Enumval =>
            Enum.GetValues(typeof(Model.Task.Status)).Cast<Model.Task.Status>().ToList();

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (SessionStudent == null)
                SessionStudent = DataSource.Students.Instance.UserStudent;

            if(Tasks == null)
                Tasks = new ObservableCollection<Model.Task>(await DataSource.Students.Instance.GetTasks(SessionStudent));

            await Task.CompletedTask;
        }
    }
}
