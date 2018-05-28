using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using StudentTask.Model.Annotations;

namespace StudentTask.Model
{
    /// <summary>
    ///     Represents a student.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class User : INotifyPropertyChanged
    {
        /// <summary>
        ///     Enum that defines the usergroup.
        /// </summary>
        public enum UserGroup
        {
            /// <summary>
            ///     The student
            /// </summary>
            Student,

            /// <summary>
            ///     The admin
            /// </summary>
            Admin,

            /// <summary>
            ///     The teacher
            /// </summary>
            Teacher
        }

        /// <summary>
        ///     Gets or sets the email.
        /// </summary>
        /// <value>
        ///     The email.
        /// </value>
        private string _email;

        /// <summary>
        ///     Gets or sets the first name.
        /// </summary>
        /// <value>
        ///     The first name.
        /// </value>
        private string _firstName;

        /// <summary>
        ///     Gets or sets the last name.
        /// </summary>
        /// <value>
        ///     The last name.
        /// </value>
        private string _lastName;

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        /// <value>
        ///     The password.
        /// </value>
        private string _password;

        /// <summary>
        ///     Gets or sets the username.
        /// </summary>
        /// <value>
        ///     The username.
        /// </value>
        private string _username;

        /// <summary>
        ///     Gets or sets the username.
        /// </summary>
        /// <value>
        ///     The username.
        /// </value>
        [Key]
        public string Username
        {
            get => _username;
            set
            {
                if (SetField(ref _username, value))
                    OnPropertyChanged(nameof(IsValid));
            }
        }

        /// <summary>
        ///     Gets or sets the email.
        /// </summary>
        /// <value>
        ///     The email.
        /// </value>
        [Required]
        public string Email
        {
            get => _email;
            set
            {
                if (SetField(ref _email, value))
                    OnPropertyChanged(nameof(IsValid));
            }
        }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        /// <value>
        ///     The password.
        /// </value>
        [Required]
        public string Password
        {
            get => _password;
            set
            {
                {
                    if (SetField(ref _password, value))
                        OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        /// <summary>
        ///     Gets or sets the first name.
        /// </summary>
        /// <value>
        ///     The first name.
        /// </value>
        [Required]
        public string FirstName
        {
            get => _firstName;
            set
            {
                {
                    if (SetField(ref _firstName, value))
                    {
                        OnPropertyChanged(nameof(IsValid));
                        OnPropertyChanged(nameof(FullName));
                    }
                }
            }
        }

        /// <summary>
        ///     Gets or sets the last name.
        /// </summary>
        /// <value>
        ///     The last name.
        /// </value>
        [Required]
        public string LastName
        {
            get => _lastName;
            set
            {
                {
                    if (SetField(ref _lastName, value))
                    {
                        OnPropertyChanged(nameof(IsValid));
                        OnPropertyChanged(nameof(FullName));
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the full name.
        /// </summary>
        /// <value>
        ///     The full name.
        /// </value>
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        ///     Gets or sets the tasks.
        /// </summary>
        /// <value>
        ///     The tasks.
        /// </value>
        public List<Task> Tasks { get; set; }

        /// <summary>
        ///     Gets or sets the courses.
        /// </summary>
        /// <value>
        ///     The courses.
        /// </value>
        public List<Course> Courses { get; set; }

        /// <summary>
        ///     Gets or sets the group usergroup.
        /// </summary>
        /// <value>
        ///     The group usergroup.
        /// </value>
        public UserGroup GroupUserGroup { get; set; }

        /// <summary>
        ///     Returns true if ... is valid.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) &&
                               !string.IsNullOrEmpty(Username)
                               && !string.IsNullOrEmpty(Email);

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