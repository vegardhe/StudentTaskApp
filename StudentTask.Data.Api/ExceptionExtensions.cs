using StudentTask.Data.Access;
using StudentTask.Model;
using System;
using System.Threading.Tasks;

namespace StudentTask.Data.Api
{
    /// <summary>
    /// Extension methods for Exceptions.
    /// </summary>
    public static class ExceptionExtensions
    {

        /// <summary>
        /// Logs the specified error to database.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="db">The database.</param>
        /// <returns></returns>
        public static async Task<Exception> Log(this Exception ex, StudentTaskContext db)
        {
            var logElement = new LogElement
            {
                Message = ex.Message,
                Type = ex.GetType().Name,
                Source = ex.StackTrace
            };
            db.LogElements.Add(logElement);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                // ignored
            }
            return ex;
        }
    }
}
