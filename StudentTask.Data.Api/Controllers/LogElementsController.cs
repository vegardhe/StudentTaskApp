using StudentTask.Data.Access;
using StudentTask.Model;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace StudentTask.Data.Api.Controllers
{
    public class LogElementsController : ApiController
    {
        private StudentTaskContext db = new StudentTaskContext();

        // POST: api/LogElements
        [ResponseType(typeof(LogElement))]
        public async Task<IHttpActionResult> PostLogElement(LogElement logElement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.LogElements.Add(logElement);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = logElement.LogElementId }, logElement);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}