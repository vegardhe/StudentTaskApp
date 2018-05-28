using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using StudentTask.Data.Access;
using StudentTask.Model;

[assembly: CLSCompliant(false)]

namespace StudentTask.Data.Api.Controllers
{
    /// <summary>
    ///     CRUD operations for courses.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class CoursesController : ApiController
    {
        /// <summary>
        ///     The database
        /// </summary>
        private readonly StudentTaskContext _db = new StudentTaskContext();

        // GET: api/Courses
        /// <summary>
        ///     Gets the courses.
        /// </summary>
        /// <returns></returns>
        public IQueryable<Course> GetCourses()
        {
            return _db.Courses;
        }

        // GET: api/Courses/5
        /// <summary>
        ///     Gets the course.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [ResponseType(typeof(Course))]
        public async Task<IHttpActionResult> GetCourse(int id)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        // GET: api/Courses/5/Resources
        /// <summary>
        ///     Gets the resources.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Courses/{courseId}/Resources")]
        [ResponseType(typeof(Resource))]
        public async Task<IHttpActionResult> GetResources(int courseId)
        {
            var result = await _db.Courses
                .Where(c => c.CourseId == courseId)
                .SelectMany(c => c.Resources)
                .ToListAsync();
            return Ok(result);
        }

        // GET: api/Courses/5/Exercises
        /// <summary>
        ///     Gets the exercises.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Courses/{courseId}/Exercises")]
        [ResponseType(typeof(Exercise))]
        public async Task<IHttpActionResult> GetExercises(int courseId)
        {
            var result = await _db.Courses
                .Where(c => c.CourseId == courseId)
                .SelectMany(c => c.Exercises)
                .ToListAsync();
            return Ok(result);
        }

        // PUT: api/Courses/5
        /// <summary>
        ///     Puts the course.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="course">The course.</param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCourse(int id, Course course)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != course.CourseId) return BadRequest();

            _db.Entry(course).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CourseExists(id)) return NotFound();
                await ex.Log(_db);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Courses
        /// <summary>
        ///     Posts the course.
        /// </summary>
        /// <param name="course">The course.</param>
        /// <returns></returns>
        [ResponseType(typeof(Course))]
        public async Task<IHttpActionResult> PostCourse(Course course)
        {
            var user = course.Users[0].Username;
            course.Users = null;
            _db.Courses.Add(course);
            await _db.SaveChangesAsync();

            try
            {
                using (var conn = new SqlConnection(_db.Database.Connection.ConnectionString))
                {
                    var cmd = new SqlCommand("INSERT INTO UserCourse VALUES (@Username, @CourseId);", conn);
                    cmd.Parameters.AddWithValue("@Username", user);
                    cmd.Parameters.AddWithValue("@CourseId", course.CourseId);

                    conn.Open();

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                await ex.Log(_db);
                return InternalServerError();
            }

            return CreatedAtRoute("DefaultApi", new {id = course.CourseId}, course);
        }

        // POST api/Courses/courseId/Resources
        /// <summary>
        ///     Posts the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="courseId">The course identifier.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Courses/{courseId}/Resources")]
        [ResponseType(typeof(Resource))]
        public async Task<IHttpActionResult> PostResource(Resource resource, int courseId)
        {
            _db.Resources.Add(resource);
            await _db.SaveChangesAsync();
            var course = await _db.Courses.FindAsync(courseId);
            if (course != null) course.Resources = new List<Resource> {resource};
            await _db.SaveChangesAsync();
            return CreatedAtRoute("DefaultApi", new {controller = "resources", id = resource.ResourceId}, resource);
        }

        // POST: api/Courses/5/Exercises
        /// <summary>
        ///     Posts the exercise.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <param name="exercise">The exercise.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Courses/{courseId}/Exercises")]
        [ResponseType(typeof(Exercise))]
        public async Task<IHttpActionResult> PostExercise(int courseId, Exercise exercise)
        {
            _db.Exercises.Add(exercise);
            await _db.SaveChangesAsync();
            var course = await _db.Courses.FindAsync(courseId);
            if (course != null) course.Exercises = new List<Exercise> {exercise};
            await _db.SaveChangesAsync();
            return CreatedAtRoute("DefaultApi", new {controller = "exercise", id = exercise.TaskId}, exercise);
        }

        // POST: api/Courses/5/Users/username
        /// <summary>
        ///     Posts the user to course.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Courses/{courseId}/users/{username}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PostUserToCourse(int courseId, string username)
        {
            try
            {
                using (var conn = new SqlConnection(_db.Database.Connection.ConnectionString))
                {
                    var cmd = new SqlCommand("INSERT INTO UserCourse VALUES (@Username, @CourseId);", conn);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@CourseId", courseId);

                    await conn.OpenAsync();

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                await ex.Log(_db);
                return InternalServerError();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Courses/5
        /// <summary>
        ///     Deletes the course.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [ResponseType(typeof(Course))]
        public async Task<IHttpActionResult> DeleteCourse(int id)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null) return NotFound();

            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();

            return Ok(course);
        }

        /// <summary>
        ///     Releases the unmanaged resources that are used by the object and, optionally, releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     true to release both managed and unmanaged resources; false to release only unmanaged
        ///     resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        ///     Courses the exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        private bool CourseExists(int id)
        {
            return _db.Courses.Count(e => e.CourseId == id) > 0;
        }
    }
}