using StudentTask.Data.Access;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Task = StudentTask.Model.Task;

namespace StudentTask.Data.Api.Controllers
{
    /// <summary>
    /// CRUD operations for Tasks.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class TasksController : ApiController
    {
        /// <summary>
        /// The database
        /// </summary>
        private StudentTaskContext db = new StudentTaskContext();

        // GET: api/Tasks
        /// <summary>
        /// Gets the tasks.
        /// </summary>
        /// <returns></returns>
        public IQueryable<Task> GetTasks() => db.Tasks;

        // GET: api/Tasks/5
        /// <summary>
        /// Gets the task.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [ResponseType(typeof(Task))]
        public async Task<IHttpActionResult> GetTask(int id)
        {
            var task = await db.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        // PUT: api/Tasks/5
        /// <summary>
        /// Puts the task.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTask(int id, Task task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != task.TaskId)
            {
                return BadRequest();
            }

            db.Entry(task).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }

                await ex.Log(db);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Tasks
        /// <summary>
        /// Posts the task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        [ResponseType(typeof(Task))]
        public async Task<IHttpActionResult> PostTask(Task task)
        {
            var owner = task.Users[0].Username;
            task.Users = null;
            db.Tasks.Add(task);
            await db.SaveChangesAsync();

            try
            {
                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    var cmd = new SqlCommand("INSERT INTO UserTask VALUES (@Username, @TaskId);",
                        conn);
                    cmd.Parameters.AddWithValue("@Username", owner);
                    cmd.Parameters.AddWithValue("@TaskId", task.TaskId);

                    conn.Open();

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                await ex.Log(db);
                return InternalServerError();
            }
            return CreatedAtRoute("DefaultApi", new { id = task.TaskId }, task);
        }

        // DELETE: api/Tasks/5
        /// <summary>
        /// Deletes the task.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [ResponseType(typeof(Task))]
        public async Task<IHttpActionResult> DeleteTask(int id)
        {
            var task = await db.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            db.Tasks.Remove(task);
            await db.SaveChangesAsync();

            return Ok(task);
        }

        /// <summary>
        /// Releases the unmanaged resources that are used by the object and, optionally, releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Tasks the exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        private bool TaskExists(int id)
        {
            return db.Tasks.Count(e => e.TaskId == id) > 0;
        }
    }
}