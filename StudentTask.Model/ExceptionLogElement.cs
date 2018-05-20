namespace StudentTask.Model
{
    /// <summary>
    /// Represents an exception log element.
    /// </summary>
    /// <seealso cref="StudentTask.Model.LogElement" />
    public class ExceptionLogElement : LogElement
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public string Source { get; set; }
    }
}
