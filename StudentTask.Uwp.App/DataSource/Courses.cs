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

[assembly: CLSCompliant(false)]

namespace StudentTask.Uwp.App.DataSource
{
    /// <summary>
    /// Database interaction for courses.
    /// </summary>
    public class Courses
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static Courses Instance { get; } = new Courses();

        /// <summary>
        /// The base URI
        /// </summary>
        private const string BaseUri = "http://localhost:52988/api/";

        /// <summary>
        /// The client
        /// </summary>
        private HttpClient _client;

        /// <summary>
        /// Prevents a default instance of the <see cref="Courses"/> class from being created.
        /// </summary>
        private Courses()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUri)
            };
        }
        /// <summary>
        /// Gets the user courses.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">Request failed</exception>
        public async Task<Course[]> GetUserCourses(User user)
        {
            string json;
            try
            {
                json = await _client.GetStringAsync($"users\\{user.Username}/courses").ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException("Request failed", e);
            }
            var courses = JsonConvert.DeserializeObject<Course[]>(json);
            Users.Instance.SessionUser.Courses = courses.ToList();
            return courses;
        }

        /// <summary>
        /// Gets the course resources.
        /// </summary>
        /// <param name="course">The course.</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">Request failed</exception>
        public async Task<Resource[]> GetCourseResources(Course course)
        {
            string json;
            try
            {
                json = await _client.GetStringAsync($"courses\\{course.CourseId}/resources")
                    .ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException("Request failed", e);
            }
            var resources = JsonConvert.DeserializeObject<Resource[]>(json);
            var resourceList = resources.ToList();
            course.Resources = resourceList;
            return resources;
        }

        /// <summary>
        /// Gets the course exercises.
        /// </summary>
        /// <param name="course">The selected course.</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">Request failed</exception>
        public async Task<Exercise[]> GetCourseExercises(Course course)
        {
            string json;
            try
            {
                json = await _client.GetStringAsync($"courses\\{course.CourseId}/exercises")
                    .ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException("Request failed", e);
            }

            var exercises = JsonConvert.DeserializeObject<Exercise[]>(json);
            var exerciseList = exercises.ToList();
            course.Exercises = exerciseList;
            return exercises;
        }

        /// <summary>
        /// Adds the course.
        /// </summary>
        /// <param name="course">The new course.</param>
        /// <returns></returns>
        /// <exception cref="WebException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public async Task<Course> AddCourse(Course course)
        {
            var postBody = JsonConvert.SerializeObject(course);
            var response = await _client
                .PostAsync("courses", new StringContent(postBody, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new WebException();
                throw new InvalidDataException();
            }
                
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Course>(responseBody);
        }

        /// <summary>
        /// Updates the course.
        /// </summary>
        /// <param name="selectedCourse">The selected course.</param>
        /// <returns></returns>
        public async Task<bool> UpdateCourse(Course selectedCourse)
        {
            var putBody = JsonConvert.SerializeObject(selectedCourse);
            var response = await _client.PutAsync($"courses\\{selectedCourse.CourseId}",
                new StringContent(putBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Deletes the course.
        /// </summary>
        /// <param name="course">The course.</param>
        /// <returns></returns>
        public async Task<bool> DeleteCourse(Course course)
        {
            var response = await _client.DeleteAsync($"courses\\{course.CourseId}").ConfigureAwait(false);
            return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Adds the exercise.
        /// </summary>
        /// <param name="exercise">The exercise.</param>
        /// <param name="course">The course.</param>
        /// <returns></returns>
        /// <exception cref="WebException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public async Task<Exercise> AddExercise(Exercise exercise, Course course)
        {
            var postBody = JsonConvert.SerializeObject(exercise);
            var response = await _client
                .PostAsync($"courses\\{course.CourseId}/Exercises",
                    new StringContent(postBody, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new WebException();
                throw new InvalidDataException();
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Exercise>(responseBody);
        }

        /// <summary>
        /// Adds the user to course.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="course">The course.</param>
        /// <returns></returns>
        public async Task<bool> AddUserToCourse(User user, Course course)
        {
            var response = await _client.PostAsync($"courses\\{course.CourseId}/users\\{user.Username}", null)
                .ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
    }
}
