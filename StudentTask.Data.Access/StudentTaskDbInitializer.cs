using System.Data.Entity;

namespace StudentTask.Data.Access
{
    /// <summary>
    /// Reseeds the database if the model is changed.
    /// </summary>
    /// <seealso cref="System.Data.Entity.DropCreateDatabaseIfModelChanges{StudentTaskContext}" />
    public class StudentTaskDbInitializer : DropCreateDatabaseIfModelChanges<StudentTaskContext>
    {
        /// <summary>
        /// A method that should be overridden to actually add data to the context for seeding.
        /// The default implementation does nothing.
        /// </summary>
        /// <param name="context">The context to seed.</param>
        protected override void Seed(StudentTaskContext context)
        {
            //TODO: Add seed data.
        }
    }
}