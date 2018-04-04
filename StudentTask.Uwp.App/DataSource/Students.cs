using Newtonsoft.Json;
using StudentTask.Model;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StudentTask.Uwp.App.DataSource
{
    public class Students
    {
        public static Students Instance { get; } = new Students();

        public Student UserStudent { get; set; }

        private const string BaseUri = "http://localhost:52988/api/";

        private HttpClient _client;

        private Students()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUri)
            };
        }

        public async Task<Student> Login(Student student)
        {
            var postBody = JsonConvert.SerializeObject(student);
            var response = await _client
                .PostAsync("students/login", new StringContent(postBody, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync();
            var sessionStudent = JsonConvert.DeserializeObject<Student>(responseBody);
            UserStudent = sessionStudent;
            return sessionStudent;
        }
    }
}
