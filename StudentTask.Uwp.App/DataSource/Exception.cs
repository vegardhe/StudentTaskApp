using Newtonsoft.Json;
using StudentTask.Model;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StudentTask.Uwp.App.DataSource
{
    public class Exception
    {
        public static Exception Instance { get; } = new Exception();

        private const string BaseUri = "http://localhost:52988/api/";

        private HttpClient _client;

        private Exception()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUri)
            };
        }

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
