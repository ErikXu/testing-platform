using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Mongo;
using WebApi.Mongo.Entities;

namespace WebApi.Controllers
{
    [Route("api/api-tasks")]
    [ApiController]
    public class ApiTasksController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;

        public ApiTasksController(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        /// <summary>
        /// Get api task list
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var list = await _mongoDbContext.Collection<ApiTask>()
                                            .Find(new BsonDocument())
                                            .Sort(Builders<ApiTask>.Sort.Descending(n => n.CreationTime))
                                            .ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Get api task by id
        /// </summary>
        [HttpGet("{id}", Name = "GetApiTask")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var apiTask = await _mongoDbContext.Collection<ApiTask>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();
            if (apiTask == null)
            {
                return NotFound();
            }

            return Ok(apiTask);
        }

        /// <summary>
        /// Get stress task report by id
        /// </summary>
        [HttpGet("{id}/report")]
        public async Task<IActionResult> GetReport([FromRoute] string id)
        {
            var apiTask = await _mongoDbContext.Collection<ApiTask>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();
            if (apiTask == null)
            {
                return NotFound();
            }

            var report = new ApiTaskReport
            {
                ApiItems = new List<ApiItem>(),
                ReportItems = new List<ApiTaskReportItem>()
            };

            if (apiTask.Result != null)
            {
                var index = 0;
                foreach (var item in apiTask.Result.Collection.Item)
                {
                    var execution = apiTask.Result.Run.Executions[index++];
                    var apiItem = new ApiItem
                    {
                        Name = item.Name,
                        Method = item.Request.Method,
                        Status = execution.Response.Status,
                        Code = execution.Response.Code,
                        ResponseTime = execution.Response.ResponseTime,
                        ResponseSize = execution.Response.ResponseSize
                    };

                    var host = string.Join('.', item.Request.Url.Host);
                    var path = string.Join('/', item.Request.Url.Path);
                    apiItem.Url = $"{item.Request.Url.Protocol}://{host}/{path}";

                    report.ApiItems.Add(apiItem);
                }

                report.ReportItems.Add(GenerateItem("Iterations", apiTask.Result.Run.Stats.Iterations));
                report.ReportItems.Add(GenerateItem("Items", apiTask.Result.Run.Stats.Items));
                report.ReportItems.Add(GenerateItem("Scripts", apiTask.Result.Run.Stats.Scripts));
                report.ReportItems.Add(GenerateItem("Prerequests", apiTask.Result.Run.Stats.Prerequests));
                report.ReportItems.Add(GenerateItem("Requests", apiTask.Result.Run.Stats.Requests));
                report.ReportItems.Add(GenerateItem("Tests", apiTask.Result.Run.Stats.Tests));
                report.ReportItems.Add(GenerateItem("Assertions", apiTask.Result.Run.Stats.Assertions));
                report.ReportItems.Add(GenerateItem("TestScripts", apiTask.Result.Run.Stats.TestScripts));
                report.ReportItems.Add(GenerateItem("PrerequestScripts", apiTask.Result.Run.Stats.PrerequestScripts));
            }

            return Ok(report);
        }

        /// <summary>
        /// Get api task scene by id
        /// </summary>
        [HttpGet("{id}/scene")]
        public async Task<IActionResult> GetScene([FromRoute] string id)
        {
            var task = await _mongoDbContext.Collection<ApiTask>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();
            if (task == null)
            {
                return NotFound();
            }

            var scene = await _mongoDbContext.Collection<ApiScene>().Find(n => n.Id == task.SceneId).SingleOrDefaultAsync();
            if (scene == null)
            {
                return NotFound();
            }

            return Ok(scene);
        }

        /// <summary>
        /// Create api task
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string sceneId)
        {
            var apiScene = await _mongoDbContext.Collection<ApiScene>()
                                             .Find(n => n.Id == new ObjectId(sceneId))
                                             .SingleOrDefaultAsync();

            if (apiScene == null)
            {
                return NotFound();
            }

            var apiTask = new ApiTask
            {
                SceneId = apiScene.Id,
                SceneName = apiScene.Name,
                Collection = apiScene.Collection,
                Environment = apiScene.Environment,
                Status = ApiTaskStatus.Queue,
                From = ApiTaskFrom.Console,
                CreationTime = DateTime.UtcNow
            };

            await _mongoDbContext.Collection<ApiTask>().InsertOneAsync(apiTask);

            return CreatedAtRoute("GetApiTask", new { id = apiTask.Id.ToString() }, apiTask);
        }

        private ApiTaskReportItem GenerateItem(string name, NewmanStatsItems statsItem)
        {
            var item = new ApiTaskReportItem
            {
                Name = name,
                Total = statsItem.Total,
                Pending = statsItem.Pending,
                Failed = statsItem.Failed
            };

            return item;
        }
    }
}
