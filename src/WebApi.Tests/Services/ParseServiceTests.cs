using WebApi.Services;
using Xunit;

namespace WebApi.Tests.Services
{
    public class ParseServiceTests
    {
        private readonly IParseService _parseService;

        public ParseServiceTests()
        {
            _parseService = new ParseService();
        }

        [Fact]
        public void ParseStressTestResult_Normal()
        {
            string content = @"Running 10s test @ http://127.0.0.1:5000/api/ping
  12 threads and 100 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency     4.28ms    1.96ms  40.07ms   86.64%
    Req/Sec     1.91k   314.96     3.12k    82.67%
  Latency Distribution
     50%    3.88ms
     75%    4.89ms
     90%    5.96ms
     99%   11.41ms
  229302 requests in 10.08s, 34.11MB read
Requests/sec:  22755.83
Transfer/sec:      3.39MB";

            var result = _parseService.ParseStressTestResult(content);

            Assert.Equal(22755.83, result.Qps);
            Assert.Equal(3.88, result.LatencyP50);
            Assert.Equal(4.89, result.LatencyP75);
            Assert.Equal(5.96, result.LatencyP90);
            Assert.Equal(11.41, result.LatencyP99);
            Assert.Equal(4.28, result.LatencyAvg);
            Assert.Equal(1.96, result.LatencyStd);
            Assert.Equal(40.07, result.LatencyMax);
        }

        [Fact]
        public void ParseStressTestResult_Normal_ContainsUs()
        {
            string content = @"Running 10s test @ http://127.0.0.1:5000/api/ping
  1 threads and 10 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency   487.83us  354.75us  27.55ms   98.56%
    Req/Sec    20.36k   807.98    21.98k    67.00%
  Latency Distribution
     50%  469.00us
     75%  507.00us
     90%  544.00us
     99%    0.96ms
  202569 requests in 10.00s, 30.14MB read
Requests/sec:  20256.21
Transfer/sec:      3.01MB";

            var result = _parseService.ParseStressTestResult(content);

            Assert.Equal(20256.21, result.Qps);
            Assert.Equal(0.469, result.LatencyP50);
            Assert.Equal(0.507, result.LatencyP75);
            Assert.Equal(0.544, result.LatencyP90);
            Assert.Equal(0.96, result.LatencyP99);
            Assert.Equal(0.48783, result.LatencyAvg);
            Assert.Equal(0.35475, result.LatencyStd);
            Assert.Equal(27.55, result.LatencyMax);
        }

        [Fact]
        public void ParseStressTestResult_Normal_ContainsError()
        {
            string content = @"Running 10s test @ http://www.baidu.com
  12 threads and 100 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency   161.74ms  199.79ms   1.80s    87.48%
    Req/Sec    71.96     31.66   202.00     66.41%
  Latency Distribution
     50%   49.03ms
     75%  239.96ms
     90%  409.02ms
     99%  923.30ms
  8584 requests in 10.01s, 86.13MB read
  Socket errors: connect 0, read 13, write 0, timeout 2
Requests/sec:    857.58
Transfer/sec:      8.60MB";

            var result = _parseService.ParseStressTestResult(content);

            Assert.Equal(857.58, result.Qps);
            Assert.Equal(49.03, result.LatencyP50);
            Assert.Equal(239.96, result.LatencyP75);
            Assert.Equal(409.02, result.LatencyP90);
            Assert.Equal(923.30, result.LatencyP99);
            Assert.Equal(161.74, result.LatencyAvg);
            Assert.Equal(199.79, result.LatencyStd);
            Assert.Equal(1800, result.LatencyMax);
        }
    }
}
