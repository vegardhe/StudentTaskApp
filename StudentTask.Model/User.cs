using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using StudentTask.Model.Annotations;

namespace StudentTask.Model
{
    /// <summary>
    /// Represents a student.
    /// </summary>
    public class User : INotifyPropertyChanged
    {
        /// <summary>
        /// Enum that defines the usergroup.
        /// </summary>
        public enum UserGroup
        {
            /// <summary>
            /// The student
            /// </summary>
            Student,
            /// <summary>
            /// The admin
            /// </summary>
            Admin,
            /// <summary>
            /// The teacher
            /// </summary>
            Teacher
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        private string _username;
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
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        private string _email;
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
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        private string _password;
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
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        private string _firstName;
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
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        private string _lastName;
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

        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Gets or sets the tasks.
        /// </summary>
        /// <value>
        /// The tasks.
        /// </value>
        public List<Task> Tasks { get; set; }

        /// <summary>
        /// Gets or sets the courses.
        /// </summary>
        /// <value>
        /// The courses.
        /// </value>
        public List<Course> Courses { get; set; }

        /// <summary>
        /// Gets or sets the group usergroup.
        /// </summary>
        /// <value>
        /// The group usergroup.
        /// </value>
        public UserGroup GroupUserGroup { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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

        public bool IsValid => !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) &&
                   !string.IsNullOrEmpty(Username)
                   && !string.IsNullOrEmpty(Email);
    }
}
