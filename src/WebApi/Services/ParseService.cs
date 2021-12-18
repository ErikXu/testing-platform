using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebApi.Mongo.Entities;

namespace WebApi.Services
{
    public interface IParseService
    {
        public TaskResult ParseStressTestResult(string content);
    }

    public class ParseService : IParseService
    {
        public TaskResult ParseStressTestResult(string content)
        {
            var lines = content.Split(Environment.NewLine).ToList();

            var p50Line = GetLine(lines, "50%");
            var p50 = GetValue(p50Line.Replace("50%", string.Empty));

            var p75Line = GetLine(lines, "75%");
            var p75 = GetValue(p75Line.Replace("75%", string.Empty));

            var p90Line = GetLine(lines, "90%");
            var p90 = GetValue(p90Line.Replace("90%", string.Empty));

            var p99Line = GetLine(lines, "99%");
            var p99 = GetValue(p99Line.Replace("99%", string.Empty));

            var qpsLine = GetLine(lines, "Requests/sec");
            var qps = double.Parse(qpsLine.Split(':')[1].Trim());

            var latencyLine = GetLine(lines, "Latency  ");
            latencyLine = Regex.Replace(latencyLine.Trim(), " {2,}", " ");
            var latencyArray = latencyLine.Split(' ');
            var avg = GetValue(latencyArray[1]);
            var std = GetValue(latencyArray[2]);
            var max = GetValue(latencyArray[3]);

            var result = new TaskResult
            {
                Content = content,
                Qps = qps,
                LatencyP50 = p50,
                LatencyP75 = p75,
                LatencyP90 = p90,
                LatencyP99 = p99,
                LatencyAvg = avg,
                LatencyStd = std,
                LatencyMax = max
            };

            return result;
        }

        private string GetLine(List<string> lines, string keyword)
        {
            return lines.First(n => n.Contains(keyword));
        }

        private double GetValue(string text)
        {
            if (text.Contains("us"))
            {
                return double.Parse(text.Trim().Replace("us", string.Empty)) / 1000;
            }

            if (text.Contains("ms"))
            {
                return double.Parse(text.Trim().Replace("ms", string.Empty));
            }

            if (text.Contains("s"))
            {
                return double.Parse(text.Trim().Replace("s", string.Empty)) * 1000;
            }

            throw new Exception($"Unsupported value: {text}");
        }
    }
}
