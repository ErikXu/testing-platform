using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/tests")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly ILogger<TestsController> _logger;

        public TestsController(ILogger<TestsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("get")]
        public IActionResult Get()
        {
            var result = new TestResult()
            {
                Origin = GetIp(Request),
                Url = UriHelper.GetDisplayUrl(Request),
                Queries = GetQueries(Request),
                Headers = GetHeaders(Request)
            };

            _logger.LogInformation(result.ToString());

            return Ok(result);
        }

        [HttpPost("post")]
        public IActionResult Post([FromBody] dynamic body)
        {
            var result = new TestResult()
            {
                Origin = GetIp(Request),
                Url = UriHelper.GetDisplayUrl(Request),
                Queries = GetQueries(Request),
                Headers = GetHeaders(Request),
                Body = body
            };

            _logger.LogInformation(result.ToString());

            return Ok(result);
        }

        [HttpPatch("patch")]
        public IActionResult Patch([FromBody] dynamic body)
        {
            var result = new TestResult()
            {
                Origin = GetIp(Request),
                Url = UriHelper.GetDisplayUrl(Request),
                Queries = GetQueries(Request),
                Headers = GetHeaders(Request),
                Body = body
            };

            _logger.LogInformation(result.ToString());

            return Ok(result);
        }

        [HttpPut("put")]
        public IActionResult Put([FromBody] dynamic body)
        {
            var result = new TestResult()
            {
                Origin = GetIp(Request),
                Url = UriHelper.GetDisplayUrl(Request),
                Queries = GetQueries(Request),
                Headers = GetHeaders(Request),
                Body = body
            };

            _logger.LogInformation(result.ToString());

            return Ok(result);
        }

        [HttpDelete("delete")]
        public IActionResult Delete()
        {
            var result = new TestResult()
            {
                Origin = GetIp(Request),
                Url = UriHelper.GetDisplayUrl(Request),
                Queries = GetQueries(Request),
                Headers = GetHeaders(Request)
            };

            _logger.LogInformation(result.ToString());

            return Ok(result);
        }

        [HttpPost("form")]
        public IActionResult Form()
        {
            var result = new TestResult()
            {
                Origin = GetIp(Request),
                Url = UriHelper.GetDisplayUrl(Request),
                Queries = GetQueries(Request),
                Headers = GetHeaders(Request),
                Forms = GetForms(Request)
            };

            _logger.LogInformation(result.ToString());

            return Ok(result);
        }

        private string GetIp(HttpRequest request)
        {
            string ipAddress;

            if (request.Headers.Keys.Contains("X-Forwarded-For"))
            {
                ipAddress = request.Headers["X-Forwarded-For"];
            }
            else
            {
                ipAddress = request.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            return ipAddress;
        }

        private Dictionary<string, string> GetQueries(HttpRequest request)
        {
            return request.Query.ToDictionary(kv => kv.Key, kv => kv.Value.First());
        }

        private Dictionary<string, string> GetHeaders(HttpRequest request)
        {
            return request.Headers.ToDictionary(kv => kv.Key, kv => kv.Value.First());
        }

        private Dictionary<string, string> GetForms(HttpRequest request)
        {
            return request.Form.ToDictionary(kv => kv.Key, kv => kv.Value.First());
        }
    }
}
