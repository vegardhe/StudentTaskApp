using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using StudentTask.Model;
using StudentTask.Uwp.App.DataSource;
using Template10.Mvvm;
using Exception = System.Exception;
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
            try
            {
                if (await Tasks.Instance.DeleteTask((Model.Task) parameter))
                    _viewModel.Tasks.Remove((Model.Task) parameter);
                else
                    await new MessageDialog("Failed to delete task", "Error").ShowAsync();
            }
            catch (Exception e)
            {
                await e.Log();
                await e.Display("Failed to delete task.");
            }
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

                try
                {
                    Tasks = new ObservableCollection<Model.Task>(await DataSource.Tasks.Instance.GetTasks(SessionUser));
                }
                catch (HttpRequestException ex)
                {
                    await ex.Display("Failed to establish database connection.");
                    await ex.Log();
                }
                catch (Exception ex)
                {
                    await ex.Display("Failed to get tasks.");
                    await ex.Log();
                }

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
