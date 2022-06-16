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
          
            content.AppendLine("------ Headers ------");
            foreach (var header in Headers)
            {
                content.AppendLine($"{header.Key}: {header.Value}");
            }

            content.AppendLine("------ Queries ------");
            foreach (var query in Queries)
            {
                content.AppendLine($"{query.Key}: {query.Value}");
            }

            content.AppendLine("------ Forms ------");
            foreach (var form in Forms)
            {
                content.AppendLine($"{form.Key}: {form.Value}");
            }

            content.AppendLine("------ Body ------");
            content.AppendLine(Body);

            return content.ToString();
        }
    }
}
