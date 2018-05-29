using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using StudentTask.Data.Access;
using Task = StudentTask.Model.Task;

namespace StudentTask.Data.Api.Controllers
{
    /// <summary>
    ///     CRUD operations for Tasks.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class TasksController : ApiController
    {
        /// <summary>
        ///     The database
        /// </summary>
        private readonly StudentTaskContext _db = new StudentTaskContext();

        // GET: api/Tasks
        /// <summary>
        ///     Gets the tasks.
        /// </summary>
        /// <returns></returns>
        public IQueryable<Task> GetTasks()
        {
            return _db.Tasks;
        }

        // GET: api/Tasks/5
        /// <summary>
        ///     Gets the task.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [ResponseType(typeof(Task))]
        public async Task<IHttpActionResult> GetTask(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            return Ok(task);
        }

        // PUT: api/Tasks/5
        /// <summary>
        ///     Puts the task.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTask(int id, Task task)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != task.TaskId) return BadRequest();

            _db.Entry(task).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TaskExists(id)) return NotFound();

                await ex.Log(_db);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Tasks
        /// <summary>
        ///     Posts the task and adds it to the user.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        [ResponseType(typeof(Task))]
        public async Task<IHttpActionResult> PostTask(Task task)
        {
            var owner = task.Users[0].Username;
            task.Users = null;
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            try
            {
                using (var conn = new SqlConnection(_db.Database.Connection.ConnectionString))
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
                await ex.Log(_db);
                return InternalServerError();
            }

            return CreatedAtRoute("DefaultApi", new {id = task.TaskId}, task);
        }

        // DELETE: api/Tasks/5
        /// <summary>
        ///     Deletes the task.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [ResponseType(typeof(Task))]
        public async Task<IHttpActionResult> DeleteTask(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();

            return Ok(task);
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
        ///     Tasks the exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        private bool TaskExists(int id)
        {
            return _db.Tasks.Count(e => e.TaskId == id) > 0;
        }
    }
}