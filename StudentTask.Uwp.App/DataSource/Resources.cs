using StudentTask.Model;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Task = System.Threading.Tasks.Task;

namespace StudentTask.Uwp.App.DataSource
{
    public class Resources
    {
        public static Resources Instance { get; } = new Resources();

        private const string BaseUri = "http://localhost:52988/api/";

        private HttpClient _client;

        private Resources()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUri)
            };
        }

        public async Task<bool> UpdateResource(Resource resource)
        {
            var putBody = JsonConvert.SerializeObject(resource);
            var response = await _client.PutAsync($"resources\\{resource.ResourceId}",
                new StringContent(putBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<Resource> AddResource(Resource newResource, Course course)
        {
            var postBody = JsonConvert.SerializeObject(newResource);
            var response = await _client.PostAsync($"courses\\{course.CourseId}/resources",
                new StringContent(postBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Resource>(responseBody);
        }

        public async Task<bool> DeleteResource(Resource resource)
        {
            var response = await _client.DeleteAsync($"resources\\{resource.ResourceId}").ConfigureAwait(false);
            return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound;
        }
    }
}
