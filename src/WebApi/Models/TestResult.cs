using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

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

        public override string ToString()
        {
            var content = new StringBuilder();
            content.AppendLine("------ Url ------");
            content.AppendLine(Url);

            content.AppendLine("------ Origin ------");
            content.AppendLine(Origin);
          
            if (Headers != null && Headers.Count > 0)
            {
                content.AppendLine("------ Headers ------");
                foreach (var header in Headers)
                {
                    content.AppendLine($"{header.Key}: {header.Value}");
                }
            }

            if (Queries != null && Queries.Count > 0)
            {
                content.AppendLine("------ Queries ------");
                foreach (var query in Queries)
                {
                    content.AppendLine($"{query.Key}: {query.Value}");
                }
            }

            if (Forms != null && Forms.Count > 0)
            {
                content.AppendLine("------ Forms ------");
                foreach (var form in Forms)
                {
                    content.AppendLine($"{form.Key}: {form.Value}");
                }
            }

            if (Body != null)
            {
                content.AppendLine("------ Body ------");
                content.AppendLine(JsonConvert.SerializeObject(Body));
            }

            return content.ToString();
        }
    }
}
