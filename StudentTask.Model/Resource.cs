using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using StudentTask.Model.Properties;

namespace StudentTask.Model
{
    /// <summary>
    ///     Represents a resource.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class Resource : INotifyPropertyChanged
    {
        /// <summary>
        ///     Gets or sets the link.
        /// </summary>
        /// <value>
        ///     The link.
        /// </value>
        private string _link;

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        private string _name;

        /// <summary>
        ///     Gets or sets the resource identifier.
        /// </summary>
        /// <value>
        ///     The resource identifier.
        /// </value>
        public int ResourceId { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        /// <summary>
        ///     Gets or sets the link.
        /// </summary>
        /// <value>
        ///     The link.
        /// </value>
        public string Link
        {
            get => _link;
            set => SetField(ref _link, value);
        }


        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Sets the field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool SetField<T>(ref T field, T value,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}