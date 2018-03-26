using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentTask.Model
{
    public class Task
    {
        enum Status
        {
            Added,
            Started,
            Finished
        };

        public int TaskId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTimeOffset DueDate { get; set; }

        public int TaskStatus { get; set; }

        public string Notes { get; set; }

        public DateTimeOffset CompletedOn { get; set; }
    }
}
