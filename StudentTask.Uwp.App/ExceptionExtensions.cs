using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Popups;
using StudentTask.Model;

namespace StudentTask.Uwp.App
{
    public static class ExceptionExtensions
    {
        public static async Task<Exception> Display(this Exception ex, string msg = "")
        {
            var message = new MessageDialog(msg, "Error");
            await message.ShowAsync();
            return ex;
        }

        public static async Task<Exception> Log(this Exception ex)
        {
            var logElement = new LogElement
            {
                Message = ex.Message,
                Type = ex.GetType().Name,
                Source = ex.StackTrace
            };

            if (!await DataSource.Exception.Instance.AddExceptionLogElement(logElement))
            {
                // TODO: Log to file or event log.
            }

            return ex;
        }
    }
}
