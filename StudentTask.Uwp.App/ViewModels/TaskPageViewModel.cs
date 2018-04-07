using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;
using StudentTask.Model;
using Template10.Mvvm;
using Task = System.Threading.Tasks.Task;

namespace StudentTask.Uwp.App.ViewModels
{
    public class DeleteTaskCommand : ICommand
    {
        private TaskPageViewModel _viewModel;

        public DeleteTaskCommand(TaskPageViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => parameter != null;

        public async void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;
            if (await DataSource.Tasks.Instance.DeleteTask((Model.Task) parameter))
                _viewModel.Tasks.Remove((Model.Task) parameter);
        }
    }

    public class TaskPageViewModel : ViewModelBase
    {

        public TaskPageViewModel()
        {
            DeleteTaskCommand = new DeleteTaskCommand(this);
        }

        public Student SessionStudent { get; set; }

        public ObservableCollection<Model.Task> Tasks { get; set; }

        public static List<Model.Task.Status> Enumval =>
            Enum.GetValues(typeof(Model.Task.Status)).Cast<Model.Task.Status>().ToList();

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (SessionStudent == null)
                SessionStudent = DataSource.Students.Instance.UserStudent;

            if(Tasks == null)
                Tasks = new ObservableCollection<Model.Task>(await DataSource.Tasks.Instance.GetTasks(SessionStudent));

            await Task.CompletedTask;
        }

        public ICommand DeleteTaskCommand { get; set; }
    }
}
