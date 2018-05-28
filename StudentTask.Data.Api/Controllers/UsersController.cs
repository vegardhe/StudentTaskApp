using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using StudentTask.Data.Access;
using StudentTask.Model;

namespace StudentTask.Data.Api.Controllers
{
    /// <summary>
    ///     CRUD operations for Users.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class UsersController : ApiController
    {
        /// <summary>
        ///     The database
        /// </summary>
        private readonly StudentTaskContext _db = new StudentTaskContext();

        // GET: api/users
        /// <summary>
        ///     Gets the users.
        /// </summary>
        /// <returns></returns>
        public IQueryable<User> GetUsers()
        {
            // Removing passwords from return values.
            var users = _db.Users;
            foreach (var user in users)
                user.Password = null;

            return users;
        }

        // GET: api/users/username/tasks
        /// <summary>
        ///     Gets the tasks.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Users/{username}/tasks")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetTasks(string username)
        {
            var query = await (from tasks in _db.Tasks
                where tasks.Users.Any(s => s.Username == username)
                select tasks).ToListAsync();
            return Ok(query);
        }

        // GET: api/users/username/courses
        /// <summary>
        ///     Gets the courses.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Users/{username}/courses")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetCourses(string username)
        {
            var query = await (from courses in _db.Courses
                where courses.Users.Any(u => u.Username == username)
                select courses).ToListAsync();
            return Ok(query);
        }

        // PUT: api/Users/5
        /// <summary>
        ///     Puts the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(string id, User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != user.Username) return BadRequest();

            _db.Entry(user).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!UserExists(id)) return NotFound();

                await ex.Log(_db);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        /// <summary>
        ///     Posts the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            user.Password = PasswordEncryption.Encrypt(user.Password);

            _db.Users.Add(user);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Username)) return Conflict();

                throw;
            }

            user.Password = null;
            return CreatedAtRoute("DefaultApi", new {id = user.Username}, user);
        }

        // POST: api/Users/Login
        /// <summary>
        ///     Users the log on.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Users/Login")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> UserLogOn(User user)
        {
            var dbUser = await _db.Users.FindAsync(user.Username);
            if (dbUser == null)
                return NotFound();

            if (!PasswordEncryption.Verify(user.Password, dbUser.Password))
                return BadRequest();

            dbUser.Password = null;
            return Ok(dbUser);
        }

        // DELETE: api/Users/5
        /// <summary>
        ///     Deletes the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            user.Password = null;
            return Ok(user);
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
        ///     Users the exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        private bool UserExists(string id)
        {
            return _db.Users.Count(e => e.Username == id) > 0;
        }
    }
}