using StudentTask.Data.Access;
using StudentTask.Model;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace StudentTask.Data.Api.Controllers
{
    public class UsersController : ApiController
    {
        private StudentTaskContext db = new StudentTaskContext();

        // GET: api/Users
        public IQueryable<User> GetUsers() => db.Users;

        // TODO: Make api not return passwords.
        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUser(string id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // GET: api/users/username/tasks
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
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            return CreatedAtRoute("DefaultApi", new { id = user.Username }, user);
        }

        // POST: api/Users/Login
        [HttpPost]
        [Route("api/Users/Login")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> UserLogOn(User user)
        {
            var dbUser = await db.Users.FindAsync(user.Username);
            if (dbUser == null)
                return NotFound();

            if (dbUser.Password == user.Password)
                return Ok(dbUser);

            return InternalServerError();
        }

        // DELETE: api/Users/5
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

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(string id)
        {
            return db.Users.Count(e => e.Username == id) > 0;
        }
    }
}