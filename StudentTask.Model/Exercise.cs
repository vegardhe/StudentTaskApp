using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentTask.Model
{
    public class Exercise : Task
    {
        public Course Course { get; set; }
    }
}
