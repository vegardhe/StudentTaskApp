using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace StudentTask.Uwp.App
{
    /// <summary>
    ///     Extension methods for exception class.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        ///     Displays the specified message in a message dialog.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static async Task<Exception> Display(this Exception ex, string message)
        {
            var messageDialog = new MessageDialog(message, "Error");
            await messageDialog.ShowAsync();
            return ex;
        }

        /// <summary>
        ///     Logs the specified exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns></returns>
        public static async Task<Exception> Log(this Exception ex)
        {
            await ErrorLogging.LogToDb(ex);
            return ex;
        }
    }
}