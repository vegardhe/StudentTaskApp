using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace StudentTask.Uwp.App
{
    /// <summary>
    /// Extension methods for exception class.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Displays the specified MSG.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="msg">The MSG.</param>
        /// <returns></returns>
        public static async Task<Exception> Display(this Exception ex, string msg = "")
        {
            var message = new MessageDialog(msg, "Error");
            await message.ShowAsync();
            return ex;
        }

        /// <summary>
        /// Logs the specified ex.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static async Task<Exception> Log(this Exception ex)
        {
            await ErrorLogging.LogToDb(ex);
            return ex;
        }
    }
}
