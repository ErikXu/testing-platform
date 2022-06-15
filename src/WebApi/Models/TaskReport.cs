using System.Collections.Generic;

namespace WebApi.Models
{
    public class TaskReport
    {
        public List<TaskReportItem> Items { get; set; }
    }

    public class TaskReportItem
    {
        public string Name { get; set; }

        public double? Baseline { get; set; }

        public double? Previous { get; set; }

        public double? Current { get; set; }
    }
}
