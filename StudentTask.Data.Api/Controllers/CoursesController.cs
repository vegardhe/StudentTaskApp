using StudentTask.Data.Access;
using StudentTask.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace StudentTask.Data.Api.Controllers
{
    public class CoursesController : ApiController
    {
        private StudentTaskContext db = new StudentTaskContext();

        // GET: api/Courses
        public IQueryable<Course> GetCourses()
        {
            return db.Courses;
        }

        // GET: api/Courses/5
        [ResponseType(typeof(Course))]
        public async Task<IHttpActionResult> GetCourse(int id)
        {
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

        // GET: api/Courses/5/Resources
        [HttpGet]
        [Route("api/Courses/{courseId}/Resources")]
        [ResponseType(typeof(Resource))]
        public async Task<IHttpActionResult> GetResources(int courseId)
        {
            var result = await db.Courses
                .Where(c => c.CourseId == courseId)
                .SelectMany(c => c.Resources)
                .ToListAsync();
            return Ok(result);
        }

        // GET: api/Courses/5/Exercises
        [HttpGet]
        [Route("api/Courses/{courseId}/Exercises")]
        [ResponseType(typeof(Exercise))]
        public async Task<IHttpActionResult> GetExercises(int courseId)
        {
            var result = await db.Courses
                .Where(c => c.CourseId == courseId)
                .SelectMany(c => c.Exercises)
                .ToListAsync();
            return Ok(result);
        }

        // PUT: api/Courses/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCourse(int id, Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != course.CourseId)
            {
                return BadRequest();
            }

            db.Entry(course).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Courses
        [ResponseType(typeof(Course))]
        public async Task<IHttpActionResult> PostCourse(Course course)
        {
            var user = course.Users[0].Username;
            course.Users = null;
            db.Courses.Add(course);
            await db.SaveChangesAsync();

            try
            {
                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    var cmd = new SqlCommand("INSERT INTO UserCourse VALUES (@Username, @CourseId);", conn);
                    cmd.Parameters.AddWithValue("@Username", user);
                    cmd.Parameters.AddWithValue("@CourseId", course.CourseId);

                    conn.Open();

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            return CreatedAtRoute("DefaultApi", new { id = course.CourseId }, course);
        }

        // POST api/Courses/courseId/Resources
        [HttpPost]
        [Route("api/Courses/{courseId}/Resources")]
        [ResponseType(typeof(Resource))]
        public async Task<IHttpActionResult> PostResource(Resource resource, int courseId)
        {
            db.Resources.Add(resource);
            await db.SaveChangesAsync();
            var course = await db.Courses.FindAsync(courseId);
            if (course != null) course.Resources = new List<Resource> {resource};
            await db.SaveChangesAsync();
            return CreatedAtRoute("DefaultApi", new { controller = "resources", id = resource.ResourceId }, resource);
        }

        // POST: api/Courses/5/Exercises
        [HttpPost]
        [Route("api/Courses/{courseId}/Exercises")]
        [ResponseType(typeof(Exercise))]
        public async Task<IHttpActionResult> PostExercise(int courseId, Exercise exercise)
        {
            db.Exercises.Add(exercise);
            await db.SaveChangesAsync();
            var course = await db.Courses.FindAsync(courseId);
            if (course != null) course.Exercises = new List<Exercise> {exercise};
            await db.SaveChangesAsync();
            return CreatedAtRoute("DefaultApi", new { controller = "exercise", id = exercise.TaskId }, exercise);
        }

        // DELETE: api/Courses/5
        [ResponseType(typeof(Course))]
        public async Task<IHttpActionResult> DeleteCourse(int id)
        {
            Course course = await db.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            db.Courses.Remove(course);
            await db.SaveChangesAsync();

            return Ok(course);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CourseExists(int id)
        {
            return db.Courses.Count(e => e.CourseId == id) > 0;
        }
    }
}