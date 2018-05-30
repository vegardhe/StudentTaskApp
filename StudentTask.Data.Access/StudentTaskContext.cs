using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using StudentTask.Model;

[assembly: CLSCompliant(false)]

namespace StudentTask.Data.Access
{
    /// <summary>
    ///     Basic database operations for StudentTask.Model.
    /// </summary>
    /// <seealso cref="System.Data.Entity.DbContext" />
    public class StudentTaskContext : DbContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StudentTaskContext" /> class.
        /// </summary>
        public StudentTaskContext() //: base(
            //"Data Source=donau.hiof.no;Initial Catalog=vegardhe;User ID=vegardhe;Password=Uh6teB")
        {
            Configuration.ProxyCreationEnabled = false;

            Database.SetInitializer(new StudentTaskDbInitializer());
        }

        /// <summary>
        ///     Gets or sets the courses.
        /// </summary>
        /// <value>
        ///     The courses.
        /// </value>
        public virtual DbSet<Course> Courses { get; set; }

        /// <summary>
        ///     Gets or sets the exercises.
        /// </summary>
        /// <value>
        ///     The exercises.
        /// </value>
        public virtual DbSet<Exercise> Exercises { get; set; }

        /// <summary>
        ///     Gets or sets the resources.
        /// </summary>
        /// <value>
        ///     The resources.
        /// </value>
        public virtual DbSet<Resource> Resources { get; set; }

        /// <summary>
        ///     Gets or sets the users.
        /// </summary>
        /// <value>
        ///     The users.
        /// </value>
        public virtual DbSet<User> Users { get; set; }

        /// <summary>
        ///     Gets or sets the tasks.
        /// </summary>
        /// <value>
        ///     The tasks.
        /// </value>
        public virtual DbSet<Task> Tasks { get; set; }

        /// <summary>
        ///     Gets or sets the log elements.
        /// </summary>
        /// <value>
        ///     The log elements.
        /// </value>
        public virtual DbSet<LogElement> LogElements { get; set; }

        /// <summary>
        ///     This method is called when the model for a derived context has been initialized, but
        ///     before the model has been locked down and used to initialize the context.  The default
        ///     implementation of this method does nothing, but it can be overridden in a derived class
        ///     such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>
        ///     Typically, this method is called only once when the first instance of a derived context
        ///     is created.  The model for that context is then cached and is for all further instances of
        ///     the context in the app domain.  This caching can be disabled by setting the ModelCaching
        ///     property on the given ModelBuidler, but note that this can seriously degrade performance.
        ///     More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        ///     classes directly.
        /// </remarks>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null) return;
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<User>()
                .HasMany(a => a.Tasks)
                .WithMany(b => b.Users)
                .Map(m =>
                {
                    m.ToTable("UserTask");
                    m.MapLeftKey("Username");
                    m.MapRightKey("TaskId");
                });

            modelBuilder.Entity<User>()
                .HasMany(a => a.Courses)
                .WithMany(b => b.Users)
                .Map(m =>
                {
                    m.ToTable("UserCourse");
                    m.MapLeftKey("Username");
                    m.MapRightKey("CourseId");
                });

            modelBuilder.Entity<Course>()
                .HasMany(a => a.Resources)
                .WithOptional()
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Course>()
                .HasMany(a => a.Exercises)
                .WithOptional()
                .WillCascadeOnDelete(true);
        }
    }
}