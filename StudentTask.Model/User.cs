using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentTask.Model
{
    /// <summary>
    /// Represents a student.
    /// </summary>
    public class User
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
        [Key]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }

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
    }
}
