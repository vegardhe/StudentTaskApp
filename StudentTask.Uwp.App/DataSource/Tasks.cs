using Newtonsoft.Json;
using StudentTask.Model;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Task = StudentTask.Model.Task;

namespace StudentTask.Uwp.App.DataSource
{
    public class Tasks
    {
        public static Tasks Instance = new Tasks();

        private const string BaseUri = "http://localhost:52988/api/";

        private HttpClient _client;

        private Tasks()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUri)
            };
        }

        public async Task<Task[]> GetTasks(User user)
        {
            var json = await _client.GetStringAsync($"users\\{user.Username}/tasks").ConfigureAwait(false);
            var tasks = JsonConvert.DeserializeObject<Task[]>(json);
            return tasks;
        }

        public async Task<bool> DeleteTask(Task task)
        {
            var response = await _client.DeleteAsync($"tasks\\{task.TaskId}").ConfigureAwait(false);
            return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound;
        }

        public async Task<bool> UpdateTask(Task changedTask)
        {
            var putBody = JsonConvert.SerializeObject(changedTask);
            var response = await _client.PutAsync($"tasks\\{changedTask.TaskId}",
                new StringContent(putBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<Task> AddTask(Task newTask)
        {
            var postBody = JsonConvert.SerializeObject(newTask);
            var response = await _client
                .PostAsync("tasks", new StringContent(postBody, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Task>(responseBody);
        }
    }
}
