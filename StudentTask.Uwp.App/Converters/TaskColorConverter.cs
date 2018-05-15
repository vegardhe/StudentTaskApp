using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Task = StudentTask.Model.Task;

namespace StudentTask.Uwp.App.Converters
{
    public class TaskColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var task = DataSource.Users.Instance.SessionUser.Tasks.Find(t => t.TaskId == (int)value);
            
            if (task.DueDate != null && task.DueDate.Value.Date < DateTimeOffset.Now.Date && task.TaskStatus != Task.Status.Finished)
            {
                return new SolidColorBrush(Colors.Red);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
