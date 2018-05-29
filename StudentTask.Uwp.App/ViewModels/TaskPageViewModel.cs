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

namespace StudentTask.Uwp.App.ViewModels
{
    /// <summary>
    ///     ViewModel for task page.
    /// </summary>
    /// <seealso cref="System.Windows.Input.ICommand" />
    public class DeleteTaskCommand : ICommand
    {
        /// <summary>
        ///     The view model
        /// </summary>
        private readonly TaskPageViewModel _viewModel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeleteTaskCommand" /> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public DeleteTaskCommand(TaskPageViewModel viewModel)
        {
            _viewModel = viewModel;
        }
#pragma warning disable 0067
        /// <summary>
        ///     Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067
        /// <summary>
        ///     Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require data to be passed, this object can
        ///     be set to null.
        /// </param>
        /// <returns>
        ///     true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return parameter != null;
        }

        /// <summary>
        ///     Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require data to be passed, this object can
        ///     be set to null.
        /// </param>
        public async void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;
            try
            {
                if (await Tasks.Instance.DeleteTask((Task) parameter))
                    _viewModel.Tasks.Remove((Task) parameter);
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
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class TaskPageViewModel : ViewModelBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TaskPageViewModel" /> class.
        /// </summary>
        public TaskPageViewModel()
        {
            DeleteTaskCommand = new DeleteTaskCommand(this);
        }

        /// <summary>
        ///     Gets or sets the session user.
        /// </summary>
        /// <value>
        ///     The session user.
        /// </value>
        public User SessionUser { get; set; }

        /// <summary>
        ///     Gets the tasks.
        /// </summary>
        /// <value>
        ///     The tasks.
        /// </value>
        public ObservableCollection<Task> Tasks { get; private set; }

        /// <summary>
        ///     Gets or sets the active tasks.
        /// </summary>
        /// <value>
        ///     The active tasks.
        /// </value>
        public ObservableCollection<Task> ActiveTasks { get; private set; }

        /// <summary>
        ///     Gets the enumval.
        /// </summary>
        /// <value>
        ///     The enumval.
        /// </value>
        public static List<Task.Status> Enumval =>
            Enum.GetValues(typeof(Task.Status)).Cast<Task.Status>().ToList();

        /// <summary>
        ///     Gets or sets the delete task command.
        /// </summary>
        /// <value>
        ///     The delete task command.
        /// </value>
        public ICommand DeleteTaskCommand { get; set; }

        /// <summary>
        ///     Called when [navigated to asynchronous].
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public override async System.Threading.Tasks.Task OnNavigatedToAsync(object parameter, NavigationMode mode,
            IDictionary<string, object> state)
        {
            if (SessionUser == null)
                SessionUser = Users.Instance.SessionUser;

            if (Tasks == null || Users.Instance.Changed)
            {
                await GetTasks();

                ActiveTasks = new ObservableCollection<Task>();
                if (Tasks != null)
                    foreach (var task in Tasks)
                        if (task.TaskStatus != Task.Status.Finished)
                            ActiveTasks.Add(task);

                Users.Instance.Changed = false;
            }

            await System.Threading.Tasks.Task.CompletedTask;
        }

        /// <summary>
        /// Gets the tasks.
        /// </summary>
        /// <returns></returns>
        private async System.Threading.Tasks.Task GetTasks()
        {
            try
            {
                Tasks = new ObservableCollection<Task>(await DataSource.Tasks.Instance.GetTasks(SessionUser));
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
        }
    }
}