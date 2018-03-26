using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentTask.Model
{
    public class Course
    {
        public int CourseId { get; set; }

        public string Name { get; set; }

        public string CourseCode { get; set; }

        public string Information { get; set; }

        public List<Exercise> Exercises { get; set; }

        public List<Resource> Resources { get; set; }
    }
}
