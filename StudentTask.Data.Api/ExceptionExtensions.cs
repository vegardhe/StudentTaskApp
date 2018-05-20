using StudentTask.Data.Access;
using StudentTask.Model;
using System;
using System.Threading.Tasks;

namespace StudentTask.Data.Api
{
    public static class ExceptionExtensions
    {

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
            catch (Exception e)
            {
                // TODO: Write to file or event log.
            }
            return ex;
        }
    }
}
