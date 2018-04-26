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
    public class ResourcesController : ApiController
    {
        private StudentTaskContext db = new StudentTaskContext();

        // GET: api/Resources
        public IQueryable<Resource> GetResources() => db.Resources;

        // GET: api/Resources/5
        [ResponseType(typeof(Resource))]
        public async Task<IHttpActionResult> GetResource(int id)
        {
            Resource resource = await db.Resources.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }

            return Ok(resource);
        }

        // PUT: api/Resources/5
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
            catch (DbUpdateConcurrencyException)
            {
                if (!ResourceExists(id))
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

        // POST: api/Resources
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
        [ResponseType(typeof(Resource))]
        public async Task<IHttpActionResult> DeleteResource(int id)
        {
            Resource resource = await db.Resources.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }

            db.Resources.Remove(resource);
            await db.SaveChangesAsync();

            return Ok(resource);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ResourceExists(int id)
        {
            return db.Resources.Count(e => e.ResourceId == id) > 0;
        }
    }
}