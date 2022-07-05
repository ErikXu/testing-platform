using System.Collections.Generic;

namespace WebApi.Models
{
    public class ApiTaskReport
    {
        public List<ApiItem> ApiItems { get; set; }

        public List<ApiTaskReportItem> ReportItems { get; set; }
    }

    public class ApiItem
    {
        public string Name { get; set; }

        public string Method { get; set; }

        public string Url { get; set; }

        public string Status { get; set; }

        public int Code { get; set; }

        public int ResponseTime { get; set; }

        public int ResponseSize { get; set; }
    }

    public class ApiTaskReportItem
    {
        public string Name { get;set; }

        public int Total { get; set; }

        public int Pending { get; set; }

        public int Failed { get; set; }
    }
}
