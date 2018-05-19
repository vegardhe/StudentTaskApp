using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Popups;

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
    }
}
