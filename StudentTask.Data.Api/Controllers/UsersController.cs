using System;
using StudentTask.Data.Access;
using StudentTask.Model;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace StudentTask.Data.Api.Controllers
{
    /// <summary>
    /// CRUD operations for Users.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class UsersController : ApiController
    {
        /// <summary>
        /// The database
        /// </summary>
        private StudentTaskContext db = new StudentTaskContext();

        // GET: api/users
        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <returns></returns>
        public IQueryable<User> GetUsers()
        {
            // Removing passwords from return values.
            var users = db.Users;
            foreach (var user in users)
                user.Password = null;

            return users;
        }

        // GET: api/users/username/tasks
        /// <summary>
        /// Gets the tasks.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Users/{username}/tasks")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetTasks(string username)
        {
            var query = await (from tasks in db.Tasks
                where tasks.Users.Any(s => s.Username == username)
                select tasks).ToListAsync();
            return Ok(query);
        }

        // GET: api/users/username/courses
        /// <summary>
        /// Gets the courses.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Users/{username}/courses")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetCourses(string username)
        {
            var query = await (from courses in db.Courses
                where courses.Users.Any(u => u.Username == username)
                select courses).ToListAsync();
            return Ok(query);
        }

        // PUT: api/Users/5
        /// <summary>
        /// Puts the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(string id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Username)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        /// <summary>
        /// Posts the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.Password = Encrypt(user.Password);

            db.Users.Add(user);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Username))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            user.Password = null;
            return CreatedAtRoute("DefaultApi", new { id = user.Username }, user);
        }

        // POST: api/Users/Login
        /// <summary>
        /// Users the log on.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Users/Login")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> UserLogOn(User user)
        {
            var dbUser = await db.Users.FindAsync(user.Username);
            if (dbUser == null)
                return NotFound();

            if (!Verify(user.Password, dbUser.Password))
                return BadRequest();

            dbUser.Password = null;
            return Ok(dbUser);

        }

        // DELETE: api/Users/5
        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            user.Password = null;
            return Ok(user);
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
        /// Users the exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        private bool UserExists(string id)
        {
            return db.Users.Count(e => e.Username == id) > 0;
        }

        // TODO: Add these to new class?
        /// <summary>
        /// Encrypts the specified password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        private static string Encrypt(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = pbkdf2.GetBytes(20);

            var hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Verifies the specified password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="savedPasswordHash">The saved password hash.</param>
        /// <returns></returns>
        private static bool Verify(string password, string savedPasswordHash)
        {
            var hashBytes = Convert.FromBase64String(savedPasswordHash);

            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = pbkdf2.GetBytes(20);
            for (var i=0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }
    }
}