using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StudentTask.Model;

namespace StudentTask.Uwp.App.DataSource
{
    /// <summary>
    ///     Database interaction for course resources.
    /// </summary>
    public class Resources
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
        ///     Prevents a default instance of the <see cref="Resources" /> class from being created.
        /// </summary>
        private Resources()
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
        public static Resources Instance { get; } = new Resources();

        /// <summary>
        ///     Adds the resource.
        /// </summary>
        /// <param name="newResource">The new resource.</param>
        /// <param name="course">The course.</param>
        /// <returns></returns>
        /// <exception cref="WebException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public async Task<Resource> AddResource(Resource newResource, Course course)
        {
            var postBody = JsonConvert.SerializeObject(newResource);
            var response = await _client.PostAsync($"courses\\{course.CourseId}/resources",
                new StringContent(postBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new WebException();
                throw new InvalidDataException();
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Resource>(responseBody);
        }

        /// <summary>
        ///     Updates the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        public async Task<bool> UpdateResource(Resource resource)
        {
            var putBody = JsonConvert.SerializeObject(resource);
            var response = await _client.PutAsync($"resources\\{resource.ResourceId}",
                new StringContent(putBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        ///     Deletes the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        public async Task<bool> DeleteResource(Resource resource)
        {
            var response = await _client.DeleteAsync($"resources\\{resource.ResourceId}").ConfigureAwait(false);
            return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound;
        }
    }
}