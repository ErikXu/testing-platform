using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        [HttpGet("Get")]
        public IActionResult Get()
        {
            var result = new TestResult()
            {
                Origin = GetIp(Request),
                Url = UriHelper.GetDisplayUrl(Request),
                Queries = GetQueries(Request),
                Headers = GetHeaders(Request)
            };

            return Ok(result);
        }

        [HttpPost("Post")]
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

            return Ok(result);
        }

        [HttpPatch("Patch")]
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

            return Ok(result);
        }

        [HttpPut("Put")]
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

            return Ok(result);
        }

        [HttpDelete("Delete")]
        public IActionResult Delete()
        {
            var result = new TestResult()
            {
                Origin = GetIp(Request),
                Url = UriHelper.GetDisplayUrl(Request),
                Queries = GetQueries(Request),
                Headers = GetHeaders(Request)
            };

            return Ok(result);
        }

        [HttpPost("Form")]
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
