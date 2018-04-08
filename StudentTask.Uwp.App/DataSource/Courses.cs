using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Newtonsoft.Json;
using StudentTask.Model;

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

        public async Task<Course[]> GetStudentCourses(Student student)
        {
            var json = await _client.GetStringAsync($"students\\{student.Username}/courses").ConfigureAwait(false);
            var courses = JsonConvert.DeserializeObject<Course[]>(json);
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
    }
}
