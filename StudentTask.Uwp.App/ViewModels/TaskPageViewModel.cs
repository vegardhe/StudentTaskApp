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
        #pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
        #pragma warning restore 0067
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

        public User SessionUser { get; set; }

        public ObservableCollection<Model.Task> Tasks { get; private set; }

        public ObservableCollection<Model.Task> ActiveTasks { get; set; }

        public static List<Model.Task.Status> Enumval =>
            Enum.GetValues(typeof(Model.Task.Status)).Cast<Model.Task.Status>().ToList();

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (SessionUser == null)
                SessionUser = DataSource.Users.Instance.SessionUser;

            if(Tasks == null || DataSource.Users.Instance.Changed)
            {
                Tasks = new ObservableCollection<Model.Task>(await DataSource.Tasks.Instance.GetTasks(SessionUser));
                ActiveTasks = new ObservableCollection<Model.Task>();
                foreach (var task in Tasks)
                {
                    if (task.TaskStatus != Model.Task.Status.Finished)
                    {
                        ActiveTasks.Add(task);
                    }
                }
                DataSource.Users.Instance.Changed = false;
            }

            await Task.CompletedTask;
        }

        public ICommand DeleteTaskCommand { get; set; }
    }
}
