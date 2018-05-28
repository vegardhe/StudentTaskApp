using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using StudentTask.Data.Access;
using StudentTask.Model;

namespace StudentTask.Data.Api.Controllers
{
    /// <summary>
    ///     CRUD operations for log elements.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class LogElementsController : ApiController
    {
        /// <summary>
        ///     The database
        /// </summary>
        private readonly StudentTaskContext _db = new StudentTaskContext();

        // POST: api/LogElements
        /// <summary>
        ///     Posts the log element.
        /// </summary>
        /// <param name="logElement">The log element.</param>
        /// <returns></returns>
        [ResponseType(typeof(LogElement))]
        public async Task<IHttpActionResult> PostLogElement(LogElement logElement)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.LogElements.Add(logElement);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new {id = logElement.LogElementId}, logElement);
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
    }
}