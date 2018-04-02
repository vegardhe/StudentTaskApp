using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StudentTask.Model;
using Task = StudentTask.Model.Task;

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

        public async Task<Student> LoginTask(Student student)
        {
            var postBody = JsonConvert.SerializeObject(student);
            var response = await _client
                .PostAsync("students/login", new StringContent(postBody, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync();
            var sessionStudent = JsonConvert.DeserializeObject<Student>(responseBody);
            if (sessionStudent != null)
                UserStudent = sessionStudent;

            return sessionStudent;
        }

        public async Task<Task[]> GetTasks(Student student)
        {
            var json = await _client.GetStringAsync($"students\\{student.Username}/tasks").ConfigureAwait(false);
            Task[] tasks = JsonConvert.DeserializeObject<Task[]>(json);
            return tasks;
        }

        public async Task<bool> UpdateTask(Task changedTask)
        {
            var putBody = JsonConvert.SerializeObject(changedTask);
            var response = await _client.PutAsync($"tasks\\{changedTask.TaskId}",
                new StringContent(putBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
    }
}
