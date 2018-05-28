using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.Storage;
using Newtonsoft.Json;
using StudentTask.Model;
using StudentTask.Uwp.App.DataSource;
using Task = System.Threading.Tasks.Task;

namespace StudentTask.Uwp.App
{
    /// <summary>
    ///     Class for logging errors to database or files.
    /// </summary>
    public static class ErrorLogging
    {
        /// <summary>
        ///     Logs to database.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static async Task LogToDb(Exception ex)
        {
            var logElement = new LogElement
            {
                Message = ex.Message,
                ElementType = ex.GetType().Name,
                Source = ex.StackTrace
            };
            try
            {
                if (!await LogElements.Instance.AddLogElement(logElement))
                    LogToFile(logElement);
            }
            catch (Exception e)
            {
                LogToFile(new LogElement
                {
                    Message = e.Message,
                    ElementType = e.GetType().Name,
                    Source = e.StackTrace
                });
                LogToFile(logElement);
            }
        }

        /// <summary>
        ///     Logs to file.
        /// </summary>
        /// <param name="logElement">The log element.</param>
        private static async void LogToFile(LogElement logElement)
        {
            var json = JsonConvert.SerializeObject(logElement);
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "ErrorLog.json");

            try
            {
                if (!File.Exists(path))
                    using (var sw = File.CreateText(path))
                    {
                        await sw.WriteLineAsync(Package.Current.DisplayName + " - Error log:");
                    }

                using (var sw = File.AppendText(path))
                {
                    await sw.WriteLineAsync(json);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                await ex.Display("Unable to access error log file.");
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}