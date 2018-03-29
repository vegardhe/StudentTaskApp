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
    public class Students
    {
        public static Students Instance { get; } = new Students();

        private const string BaseUri = "http://localhost:52988/api/";

        private HttpClient _client;

        private Students()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUri)
            };
        }

        public async Task<Student> LoginTask(Student student)
        {
            var postBody = JsonConvert.SerializeObject(student);
            var response = await _client
                .PostAsync("students/login", new StringContent(postBody, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync();
            var seesionStudent = JsonConvert.DeserializeObject<Student>(responseBody);
            return seesionStudent;
        }
    }
}
