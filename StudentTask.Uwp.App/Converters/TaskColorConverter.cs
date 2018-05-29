using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using StudentTask.Model;
using StudentTask.Uwp.App.DataSource;

namespace StudentTask.Uwp.App.Converters
{
    /// <summary>
    ///     Changes task display color if duedate has expired.
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Data.IValueConverter" />
    public class TaskColorConverter : IValueConverter
    {
        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var task = Users.Instance.SessionUser.Tasks.Find(t => t.TaskId == (int) value);

            return IsExpired(task) ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
        }

        /// <summary>
        ///     Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Determines whether the specified task is expired.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns>
        ///     <c>true</c> if the specified task is expired; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsExpired(Task task)
        {
            return task.DueDate != null && task.DueDate.Value.Date < DateTimeOffset.Now.Date
                                        && task.TaskStatus != Task.Status.Finished;
        }
    }
}