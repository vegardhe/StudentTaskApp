using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StudentTask.Model;

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

        public async Task<bool> AddExceptionLogElement(ExceptionLogElement exceptionLogElement)
        {
            var postBody = JsonConvert.SerializeObject(exceptionLogElement);
            var response = await _client
                .PostAsync("logelements", new StringContent(postBody, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
    }
}
