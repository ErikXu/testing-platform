using System.Collections.Generic;

namespace WebApi.Models
{
    public class ApiTaskReport
    {
        public List<ApiTaskReportItem> Items { get; set; }
    }

    public class ApiTaskReportItem
    {
        public string Name { get; set; }

        public string Method { get; set; }

        public string Url { get; set; }

        public string Status { get; set; }

        public int Code { get; set; }

        public int ResponseTime { get; set; }

        public int ResponseSize { get; set; }
    }
}
