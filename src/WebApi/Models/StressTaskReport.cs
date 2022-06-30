using System.Collections.Generic;

namespace WebApi.Models
{
    public class StressTaskReport
    {
        public List<StressTaskReportItem> Items { get; set; }
    }

    public class StressTaskReportItem
    {
        public string Name { get; set; }

        public string Baseline { get; set; }

        public string Previous { get; set; }

        public string Current { get; set; }

        public int PreviousToCurrent { get; set; }

        public int BaselineToCurrent { get; set; }
    }
}
