using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Newtonsoft.Json;
using StudentTask.Model;
using Task = System.Threading.Tasks.Task;

namespace StudentTask.Uwp.App
{
    public static class ErrorLogging
    {
        public static async Task LogToDb(Exception ex)
        {
            var logElement = new LogElement
            {
                Message = ex.Message,
                Type = ex.GetType().Name,
                Source = ex.StackTrace
            };
            try
            {
                if(!await DataSource.Exception.Instance.AddLogElement(logElement))
                    LogToFile(logElement);
            }
            catch (Exception e)
            {
                LogToFile(new LogElement
                {
                    Message = e.Message,
                    Type = e.GetType().Name,
                    Source = e.StackTrace
                });
                LogToFile(logElement);
            }
            
        }

        private static async void LogToFile(LogElement logElement)
        {
            var json = JsonConvert.SerializeObject(logElement);
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "ErrorLog.json");

            try
            {
                if (!File.Exists(path))
                    using (var sw = File.CreateText(path))
                        await sw.WriteLineAsync(Package.Current.DisplayName + " - Error log:");

                using (var sw = File.AppendText(path))
                    await sw.WriteLineAsync(json);
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