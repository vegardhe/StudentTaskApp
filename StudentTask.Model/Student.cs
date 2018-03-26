using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudentTask.Model
{
    public class Student
    {
        public int StudentId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<Task> Tasks { get; set; }

        public List<Course> Courses { get; set; }
    }
}
