using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using StudentTask.Model.Properties;

namespace StudentTask.Model
{
    /// <summary>
    ///     Represents a taks.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class Task : INotifyPropertyChanged
    {
        /// <summary>
        ///     enum used for task status.
        /// </summary>
        public enum Status
        {
            /// <summary>
            ///     added task
            /// </summary>
            Added,

            /// <summary>
            ///     started task
            /// </summary>
            Started,

            /// <summary>
            ///     finished task
            /// </summary>
            Finished
        };

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        private string _description;

        /// <summary>
        ///     Gets or sets the due date.
        /// </summary>
        /// <value>
        ///     The due date.
        /// </value>
        private DateTimeOffset? _dueDate;

        /// <summary>
        ///     Gets or sets the due time.
        /// </summary>
        /// <value>
        ///     The due time.
        /// </value>
        private TimeSpan _dueTime;

        /// <summary>
        ///     Gets or sets the task status.
        /// </summary>
        /// <value>
        ///     The task status.
        /// </value>
        private Status _taskStatus;

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value>
        ///     The title.
        /// </value>
        private string _title;

        /// <summary>
        ///     Gets or sets the task identifier.
        /// </summary>
        /// <value>
        ///     The task identifier.
        /// </value>
        public int TaskId { get; set; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value>
        ///     The title.
        /// </value>
        public string Title
        {
            get => _title;
            set
            {
                if (SetField(ref _title, value))
                    OnPropertyChanged(nameof(IsValid));
            }
        }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        /// <summary>
        ///     Gets or sets the due date.
        /// </summary>
        /// <value>
        ///     The due date.
        /// </value>
        public DateTimeOffset? DueDate
        {
            get => _dueDate;
            set => SetField(ref _dueDate, value);
        }

        /// <summary>
        ///     Gets or sets the due time.
        /// </summary>
        /// <value>
        ///     The due time.
        /// </value>
        public TimeSpan DueTime
        {
            get => _dueTime;
            set => SetField(ref _dueTime, value);
        }

        /// <summary>
        ///     Gets or sets the task status.
        /// </summary>
        /// <value>
        ///     The task status.
        /// </value>
        public Status TaskStatus
        {
            get => _taskStatus;
            set
            {
                if (SetField(ref _taskStatus, value))
                    if (_taskStatus == Status.Finished)
                        CompletedOn = DateTimeOffset.Now;
            }
        }

        /// <summary>
        ///     Gets or sets the notes.
        /// </summary>
        /// <value>
        ///     The notes.
        /// </value>
        public string Notes { get; set; }

        /// <summary>
        ///     Gets or sets the completed on.
        /// </summary>
        /// <value>
        ///     The completed on.
        /// </value>
        public DateTimeOffset? CompletedOn { get; set; }

        /// <summary>
        ///     Gets or sets the users.
        /// </summary>
        /// <value>
        ///     The users.
        /// </value>
        public List<User> Users { get; set; }

        /// <summary>
        ///     Returns true if ... is valid.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => !string.IsNullOrEmpty(Title);

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
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