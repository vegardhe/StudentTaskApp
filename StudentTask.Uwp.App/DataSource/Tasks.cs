using Newtonsoft.Json;
using StudentTask.Model;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Task = StudentTask.Model.Task;

namespace StudentTask.Uwp.App.DataSource
{
    /// <summary>
    /// Database interaction for tasks.
    /// </summary>
    public class Tasks
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static Tasks Instance = new Tasks();

        /// <summary>
        /// The base URI
        /// </summary>
        private const string BaseUri = "http://localhost:52988/api/";

        /// <summary>
        /// The client
        /// </summary>
        private HttpClient _client;

        /// <summary>
        /// Prevents a default instance of the <see cref="Tasks"/> class from being created.
        /// </summary>
        private Tasks()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUri)
            };
        }

        /// <summary>
        /// Gets the tasks.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">Getting tasks failed.</exception>
        public async Task<Task[]> GetTasks(User user)
        {
            string json;
            try
            {
                json = await _client.GetStringAsync($"users\\{user.Username}/tasks").ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException("Getting tasks failed.", e);
            }
            var tasks = JsonConvert.DeserializeObject<Task[]>(json);
            Users.Instance.SessionUser.Tasks = tasks.ToList();
            return tasks;
        }

        /// <summary>
        /// Deletes the task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public async Task<bool> DeleteTask(Task task)
        {
            var response = await _client.DeleteAsync($"tasks\\{task.TaskId}").ConfigureAwait(false);
            return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Updates the task.
        /// </summary>
        /// <param name="changedTask">The changed task.</param>
        /// <returns></returns>
        public async Task<bool> UpdateTask(Task changedTask)
        {
            var putBody = JsonConvert.SerializeObject(changedTask);
            var response = await _client.PutAsync($"tasks\\{changedTask.TaskId}",
                new StringContent(putBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Adds the task.
        /// </summary>
        /// <param name="newTask">The new task.</param>
        /// <returns></returns>
        /// <exception cref="WebException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public async Task<Task> AddTask(Task newTask)
        {
            var postBody = JsonConvert.SerializeObject(newTask);
            var response = await _client
                .PostAsync("tasks", new StringContent(postBody, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new WebException();
                throw new InvalidDataException();
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdTask = JsonConvert.DeserializeObject<Task>(responseBody);
            Users.Instance.SessionUser.Tasks.Add(createdTask);
            return createdTask;
        }
    }
}
