using System.Collections.Generic;

namespace WebApi.Models
{
    public class TestResult
    {
        public Dictionary<string, string> Queries { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public string Origin { get; set; }

        public string Url { get; set; }

        public dynamic Body { get; set; }

        public Dictionary<string, string> Forms { get; set; }
    }
}
