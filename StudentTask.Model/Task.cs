using System;

namespace StudentTask.Model
{
    /// <summary>
    /// Represents a taks.
    /// </summary>
    public class Task
    {
        /// <summary>
        /// enum used for task status.
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// added task
            /// </summary>
            Added,
            /// <summary>
            /// started task
            /// </summary>
            Started,
            /// <summary>
            /// finished task
            /// </summary>
            Finished
        };

        /// <summary>
        /// Gets or sets the task identifier.
        /// </summary>
        /// <value>
        /// The task identifier.
        /// </value>
        public int TaskId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>
        /// The due date.
        /// </value>
        public DateTimeOffset DueDate { get; set; }

        /// <summary>
        /// Gets or sets the task status.
        /// </summary>
        /// <value>
        /// The task status.
        /// </value>
        public Status TaskStatus { get; set; }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>
        /// The notes.
        /// </value>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the completed on.
        /// </summary>
        /// <value>
        /// The completed on.
        /// </value>
        public DateTimeOffset CompletedOn { get; set; }
    }
}
