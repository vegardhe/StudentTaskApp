using System;
using System.Collections.Generic;
using System.Net.Http;
using Windows.UI.Xaml.Navigation;
using StudentTask.Model;
using StudentTask.Uwp.App.DataSource;
using Template10.Mvvm;
using Task = System.Threading.Tasks.Task;

namespace StudentTask.Uwp.App.ViewModels
{
    /// <summary>
    ///     ViewModel for profile page.
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class ProfilePageViewModel : ViewModelBase
    {
        /// <summary>
        ///     Gets or sets the session user.
        /// </summary>
        /// <value>
        ///     The session user.
        /// </value>
        public User SessionUser { get; set; }

        /// <summary>
        ///     Gets or sets the completed tasks.
        /// </summary>
        /// <value>
        ///     The completed tasks.
        /// </value>
        public int CompletedTasks { get; set; }

        /// <summary>
        ///     Gets or sets the completed tasks this week.
        /// </summary>
        /// <value>
        ///     The completed tasks this week.
        /// </value>
        public int CompletedTasksThisWeek { get; set; }

        /// <summary>
        ///     Gets or sets the tasks due this week.
        /// </summary>
        /// <value>
        ///     The tasks due this week.
        /// </value>
        public int TasksDueThisWeek { get; set; }

        /// <summary>
        ///     Gets or sets the tasks due today.
        /// </summary>
        /// <value>
        ///     The tasks due today.
        /// </value>
        public int TasksDueToday { get; set; }

        /// <summary>
        ///     Called when [navigated to asynchronous].
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode,
            IDictionary<string, object> state)
        {
            if (Users.Instance.SessionUser.Courses == null)
                try
                {
                    await Courses.Instance.GetUserCourses(Users.Instance.SessionUser);
                }
                catch (HttpRequestException ex)
                {
                    await ex.Log();
                    await ex.Display("Requesting courses failed.");
                }

            if (Users.Instance.SessionUser.Tasks == null)
                try
                {
                    await Tasks.Instance.GetTasks(Users.Instance.SessionUser);
                }
                catch (HttpRequestException ex)
                {
                    await ex.Display("Unable to establish database connection.");
                    await ex.Log();
                }
                catch (Exception ex)
                {
                    await ex.Log();
                    await ex.Display("Unable to get tasks.");
                }

            SessionUser = Users.Instance.SessionUser;

            foreach (var task in SessionUser.Tasks)
                if (task.TaskStatus == Model.Task.Status.Finished)
                {
                    CompletedTasks++;
                    if (task.CompletedOn != null && task.CompletedOn.Value >= DateTimeOffset.Now.AddDays(-7))
                        CompletedTasksThisWeek++;
                }
                else
                {
                    if (task.DueDate == null || task.DueDate.Value.Date < DateTimeOffset.Now.Date) continue;
                    if (task.DueDate.Value.Date == DateTime.Now.Date)
                    {
                        TasksDueToday++;
                        TasksDueThisWeek++;
                    }
                    else if (task.DueDate.Value <= DateTimeOffset.Now.AddDays(7))
                    {
                        TasksDueThisWeek++;
                    }
                }
        }
    }
}