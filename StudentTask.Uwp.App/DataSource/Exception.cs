using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StudentTask.Model;

namespace StudentTask.Uwp.App.DataSource
{
    /// <summary>
    ///     Database interaction for LogElements.
    /// </summary>
    public class LogElements
    {
        /// <summary>
        ///     The base URI
        /// </summary>
        private const string BaseUri = "http://localhost:52988/api/";

        /// <summary>
        ///     The client
        /// </summary>
        private readonly HttpClient _client;

        /// <summary>
        ///     Prevents a default instance of the <see cref="LogElements" /> class from being created.
        /// </summary>
        private LogElements()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUri)
            };
        }

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static LogElements Instance { get; } = new LogElements();

        /// <summary>
        ///     Adds the log element.
        /// </summary>
        /// <param name="logElement">The log element.</param>
        /// <returns></returns>
        public async Task<bool> AddLogElement(LogElement logElement)
        {
            var postBody = JsonConvert.SerializeObject(logElement);
            var response = await _client
                .PostAsync("logelements", new StringContent(postBody, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
    }
}