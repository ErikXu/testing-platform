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
            string content = @"";


            var result = _parseService.ParseStressTestResult(content);
        }
    }
}
