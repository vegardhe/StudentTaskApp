using StudentTask.Data.Access;
using StudentTask.Model;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ExceptionHandling;

namespace StudentTask.Data.Api.Controllers
{
    /// <summary>
    /// CRUD operations for Resources.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class ResourcesController : ApiController
    {
        /// <summary>
        /// The database
        /// </summary>
        private StudentTaskContext db = new StudentTaskContext();

        // GET: api/Resources
        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <returns></returns>
        public IQueryable<Resource> GetResources() => db.Resources;

        // GET: api/Resources/5
        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [ResponseType(typeof(Resource))]
        public async Task<IHttpActionResult> GetResource(int id)
        {
            var resource = await db.Resources.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }

            return Ok(resource);
        }

        // PUT: api/Resources/5
        /// <summary>
        /// Puts the resource.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutResource(int id, Resource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != resource.ResourceId)
            {
                return BadRequest();
            }

            db.Entry(resource).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ResourceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    await ex.Log(db);
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Resources
        /// <summary>
        /// Posts the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        [ResponseType(typeof(Resource))]
        public async Task<IHttpActionResult> PostResource(Resource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Resources.Add(resource);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = resource.ResourceId }, resource);
        }

        // DELETE: api/Resources/5
        /// <summary>
        /// Deletes the resource.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [ResponseType(typeof(Resource))]
        public async Task<IHttpActionResult> DeleteResource(int id)
        {
            var resource = await db.Resources.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }

            db.Resources.Remove(resource);
            await db.SaveChangesAsync();

            return Ok(resource);
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
        /// Resources the exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        private bool ResourceExists(int id)
        {
            return db.Resources.Count(e => e.ResourceId == id) > 0;
        }
    }
}