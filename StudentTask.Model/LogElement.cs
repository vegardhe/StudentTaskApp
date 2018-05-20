using System;

namespace StudentTask.Model
{
    /// <summary>
    /// Represents a log element.
    /// </summary>
    public class LogElement
    {
        /// <summary>
        /// Gets or sets the log element identifier.
        /// </summary>
        /// <value>
        /// The log element identifier.
        /// </value>
        public int LogElementId { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
