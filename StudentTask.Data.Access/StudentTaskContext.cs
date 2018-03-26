using StudentTask.Model;
using System.Data.Entity;

namespace StudentTask.Data.Access
{
    /// <summary>
    /// Basic database operations for StudentTask.Model.
    /// </summary>
    /// <seealso cref="System.Data.Entity.DbContext" />
    public class StudentTaskContext : DbContext
    {
        /// <summary>
        /// Gets or sets the courses.
        /// </summary>
        /// <value>
        /// The courses.
        /// </value>
        public virtual DbSet<Course> Courses { get; set; }

        /// <summary>
        /// Gets or sets the exercises.
        /// </summary>
        /// <value>
        /// The exercises.
        /// </value>
        public virtual DbSet<Exercise> Exercises { get; set; }

        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>
        /// The resources.
        /// </value>
        public virtual DbSet<Resource> Resources { get; set; }

        /// <summary>
        /// Gets or sets the students.
        /// </summary>
        /// <value>
        /// The students.
        /// </value>
        public virtual DbSet<Student> Students { get; set; }

        /// <summary>
        /// Gets or sets the tasks.
        /// </summary>
        /// <value>
        /// The tasks.
        /// </value>
        public virtual DbSet<Task> Tasks { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentTaskContext"/> class.
        /// </summary>
        public StudentTaskContext()
        {
            Configuration.ProxyCreationEnabled = false;

            Database.SetInitializer(new StudentTaskDbInitializer());
        }
    }
}
