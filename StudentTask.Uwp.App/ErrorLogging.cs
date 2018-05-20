using System;
using System.IO;
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

            if (!await DataSource.Exception.Instance.AddLogElement(logElement))
                LogToFile(logElement);
        }

        private static void LogToFile(LogElement logElement)
        {
            var json = JsonConvert.SerializeObject(logElement);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "ErrorLog.txt");

            File.WriteAllText(path, json);
        }
    }
}