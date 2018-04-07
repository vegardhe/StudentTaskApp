using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using StudentTask.Model.Annotations;

namespace StudentTask.Model
{
    /// <summary>
    /// Represents a taks.
    /// </summary>
    public class Task : INotifyPropertyChanged
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
        private string _title;
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
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        private string _description;
        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>
        /// The due date.
        /// </value>
        private DateTimeOffset? _dueDate;
        public DateTimeOffset? DueDate
        {
            get => _dueDate;
            set => SetField(ref _dueDate, value);
        }

        /// <summary>
        /// Gets or sets the due time.
        /// </summary>
        /// <value>
        /// The due time.
        /// </value>
        private TimeSpan _dueTime;
        public TimeSpan DueTime
        {
            get => _dueTime;
            set => SetField(ref _dueTime, value);
        }

        /// <summary>
        /// Gets or sets the task status.
        /// </summary>
        /// <value>
        /// The task status.
        /// </value>
        private Status _taskStatus;
        public Status TaskStatus
        {
            get => _taskStatus;
            set => SetField(ref _taskStatus, value);
        }

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
        public DateTimeOffset? CompletedOn { get; set; }

        /// <summary>
        /// Gets or sets the students.
        /// </summary>
        /// <value>
        /// The students.
        /// </value>
        public List<Student> Students { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public bool IsValid => !string.IsNullOrEmpty(Title);
    }
}
