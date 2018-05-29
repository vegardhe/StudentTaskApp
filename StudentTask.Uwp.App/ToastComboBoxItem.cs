namespace StudentTask.Uwp.App
{
    /// <summary>
    /// ComboBoxItem for toast time.
    /// </summary>
    public class ToastComboBoxItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ToastComboBoxItem"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        public ToastComboBoxItem(string text, int value)
        {
            Text = text;
            Value = value;
        }

        /// <summary>
        ///     Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        private string Text { get; }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public int Value { get; }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Text;
        }
    }
}
