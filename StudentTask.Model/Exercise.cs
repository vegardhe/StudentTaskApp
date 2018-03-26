namespace StudentTask.Model
{
    /// <summary>
    /// Represents an exercise.
    /// </summary>
    /// <seealso cref="StudentTask.Model.Task" />
    public class Exercise : Task
    {
        /// <summary>
        /// Gets or sets the course.
        /// </summary>
        /// <value>
        /// The course.
        /// </value>
        public Course Course { get; set; }
    }
}
