using System;
using Windows.UI.Notifications;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;

namespace StudentTask.Uwp.App
{
    /// <summary>
    /// Allows creation of toasts for tasks.
    /// </summary>
    public static class TaskToast
    {

        /// <summary>
        /// Creates the task toast.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="timePrior">The time prior.</param>
        public static void CreateTaskToast(Model.Task task, int timePrior)
        {
            if (task?.DueDate == null) return;
            var date = task.DueDate.Value;
            var time = task.DueTime;

            var displayTime = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0);
            if(displayTime < DateTime.Now) return;
            displayTime = displayTime.AddMinutes(-timePrior);
            var toastXml = CreateXmlDocument(task, date, time);

            var toastNotification = new ScheduledToastNotification(toastXml, displayTime) { Id = $"{task.TaskId}" };

            ToastNotificationManager.CreateToastNotifier().AddToSchedule(toastNotification);
        }

        /// <summary>
        /// Creates the XML document.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="date">The date.</param>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        private static XmlDocument CreateXmlDocument(Model.Task task, DateTimeOffset date, TimeSpan time)
        {
            var dateText = date.Day + "." + date.Month + "." + date.Year;

            var toastVisual =
                $@"<visual>
                <binding template='ToastGeneric'>
                    <text>{task.Title}</text>
                    <text>Due date: {dateText}</text>
                    <text>Due time: {time}</text>
                </binding>
               </visual>";

            var toastActions =
                $@"<actions>
                    <input id='snoozeTime' type='selection' defaultInput='15'>
                        <selection id = '1' content = '1 minute'/>
                        <selection id = '15' content = '15 minutes'/>
                        <selection id = '60' content = '1 hour'/>
                        <selection id = '240' content = '4 hours'/>
                    </input>
               
                    <action
                        activationType = 'system'
                        arguments = 'snooze'
                        hint-inputId = 'snoozeTime'
                        content = '' />

                    <action
                        activationType = 'system'
                        arguments = 'dismiss'
                        content = '' />
                </actions> ";

            var toastXmlString =
                $@"<toast launch='eventId={task.TaskId}' scenario='reminder'>
                    {toastVisual}
                    {toastActions}
                </toast>";

            var toastXml = new XmlDocument();
            toastXml.LoadXml(toastXmlString);
            return toastXml;
        }

        /// <summary>
        /// Removes the task toast.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public static void RemoveTaskToast(int id)
        {
            var scheduled = ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications();

            foreach (var notification in scheduled)
            {
                if(notification.Id == id.ToString())
                    ToastNotificationManager.CreateToastNotifier().RemoveFromSchedule(notification);
            }
        }
    }
}
