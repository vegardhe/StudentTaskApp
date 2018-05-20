using Newtonsoft.Json;
using StudentTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

[assembly: CLSCompliant(false)]

namespace StudentTask.Uwp.App.DataSource
{
    public class Courses
    {
        public static Courses Instance { get; } = new Courses();

        private const string BaseUri = "http://localhost:52988/api/";

        private HttpClient _client;

        private Courses()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUri)
            };
        }

        public async Task<Course[]> GetUserCourses(User user)
        {
            var json = await _client.GetStringAsync($"users\\{user.Username}/courses").ConfigureAwait(false);
            var courses = JsonConvert.DeserializeObject<Course[]>(json);
            Users.Instance.SessionUser.Courses = courses.ToList();
            return courses;
        }

        public async Task<Resource[]> GetCourseResources(Course selectedCourse)
        {
            var json = await _client.GetStringAsync($"courses\\{selectedCourse.CourseId}/resources")
                .ConfigureAwait(false);
            var resources = JsonConvert.DeserializeObject<Resource[]>(json);
            var resourceList = new List<Resource>();
            foreach (var r in resources)
            {
                resourceList.Add(r);
            }

            selectedCourse.Resources = resourceList;
            return resources;
        }

        public async Task<Exercise[]> GetCourseExercises(Course selectedCourse)
        {
            var json = await _client.GetStringAsync($"courses\\{selectedCourse.CourseId}/exercises")
                .ConfigureAwait(false);
            var exercises = JsonConvert.DeserializeObject<Exercise[]>(json);
            var exerciseList = new List<Exercise>();
            foreach (var e in exercises)
            {
                exerciseList.Add(e);
            }

            selectedCourse.Exercises = exerciseList;
            return exercises;
        }

        public async Task<Course> AddCourse(Course newCourse)
        {
            var postBody = JsonConvert.SerializeObject(newCourse);
            var response = await _client
                .PostAsync("courses", new StringContent(postBody, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Course>(responseBody);
        }

        public async Task<bool> UpdateCourse(Course selectedCourse)
        {
            var putBody = JsonConvert.SerializeObject(selectedCourse);
            var response = await _client.PutAsync($"courses\\{selectedCourse.CourseId}",
                new StringContent(putBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCourse(Course course)
        {
            var response = await _client.DeleteAsync($"courses\\{course.CourseId}").ConfigureAwait(false);
            return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound;
        }

        public async Task<Exercise> AddExercise(Exercise exercise, Course course)
        {
            var postBody = JsonConvert.SerializeObject(exercise);
            var response = await _client
                .PostAsync($"courses\\{course.CourseId}/Exercises",
                    new StringContent(postBody, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Exercise>(responseBody);
        }

        public async Task<bool> AddUserToCourse(User user, Course course)
        {
            var response = await _client.PostAsync($"courses\\{course.CourseId}/users\\{user.Username}", null)
                .ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
    }
}
