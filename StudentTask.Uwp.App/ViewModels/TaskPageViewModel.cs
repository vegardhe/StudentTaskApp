using StudentTask.Model;
using StudentTask.Uwp.App.DataSource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;
using Template10.Mvvm;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using Exception = System.Exception;
using Task = System.Threading.Tasks.Task;

namespace StudentTask.Uwp.App.ViewModels
{
    /// <summary>
    /// ViewModel for task page.
    /// </summary>
    /// <seealso cref="System.Windows.Input.ICommand" />
    public class DeleteTaskCommand : ICommand
    {
        /// <summary>
        /// The view model
        /// </summary>
        private TaskPageViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteTaskCommand"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public DeleteTaskCommand(TaskPageViewModel viewModel)
        {
            _viewModel = viewModel;
        }
#pragma warning disable 0067
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter) => parameter != null;

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
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

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class TaskPageViewModel : ViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskPageViewModel"/> class.
        /// </summary>
        public TaskPageViewModel()
        {
            DeleteTaskCommand = new DeleteTaskCommand(this);
        }

        /// <summary>
        /// Gets or sets the session user.
        /// </summary>
        /// <value>
        /// The session user.
        /// </value>
        public User SessionUser { get; set; }

        /// <summary>
        /// Gets the tasks.
        /// </summary>
        /// <value>
        /// The tasks.
        /// </value>
        public ObservableCollection<Model.Task> Tasks { get; private set; }

        /// <summary>
        /// Gets or sets the active tasks.
        /// </summary>
        /// <value>
        /// The active tasks.
        /// </value>
        public ObservableCollection<Model.Task> ActiveTasks { get; set; }

        /// <summary>
        /// Gets the enumval.
        /// </summary>
        /// <value>
        /// The enumval.
        /// </value>
        public static List<Model.Task.Status> Enumval =>
            Enum.GetValues(typeof(Model.Task.Status)).Cast<Model.Task.Status>().ToList();

        /// <summary>
        /// Called when [navigated to asynchronous].
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (SessionUser == null)
                SessionUser = Users.Instance.SessionUser;

            if(Tasks == null || Users.Instance.Changed)
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
                if (Tasks != null)
                    foreach (var task in Tasks)
                    {
                        if (task.TaskStatus != Model.Task.Status.Finished)
                        {
                            ActiveTasks.Add(task);
                        }
                    }

                Users.Instance.Changed = false;
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets or sets the delete task command.
        /// </summary>
        /// <value>
        /// The delete task command.
        /// </value>
        public ICommand DeleteTaskCommand { get; set; }
    }
}
