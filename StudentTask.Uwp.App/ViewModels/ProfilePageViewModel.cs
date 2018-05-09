using StudentTask.Model;
using StudentTask.Uwp.App.DataSource;
using System;
using System.Collections.Generic;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
using Task = System.Threading.Tasks.Task;

namespace StudentTask.Uwp.App.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        public User SessionUser { get; set; }

        public int CompletedTasks { get; set; }

        public int CompletedTasksThisWeek { get; set; }

        public int TasksDueThisWeek { get; set; }

        public int TasksDueToday { get; set; }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (Users.Instance.SessionUser.Courses == null)
                await Courses.Instance.GetUserCourses(Users.Instance.SessionUser);
            if (Users.Instance.SessionUser.Tasks == null)
                await Tasks.Instance.GetTasks(Users.Instance.SessionUser);

            SessionUser = Users.Instance.SessionUser;

            foreach (var task in SessionUser.Tasks)
            {
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
                        TasksDueThisWeek++;
                }
            }
        }
    }
}
