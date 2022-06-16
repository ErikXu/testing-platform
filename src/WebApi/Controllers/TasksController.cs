using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Mongo;
using WebApi.Mongo.Entities;
using System.Linq;

namespace WebApi.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;

        public TasksController(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        /// <summary>
        /// Get task list
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var list = await _mongoDbContext.Collection<Mongo.Entities.Task>()
                                            .Find(new BsonDocument())
                                            .Sort(Builders<Mongo.Entities.Task>.Sort.Descending(n => n.CreationTime))
                                            .ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Get task by id
        /// </summary>
        [HttpGet("{id}", Name = "GetTask")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var task = await _mongoDbContext.Collection<Mongo.Entities.Task>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        /// <summary>
        /// Get task report by id
        /// </summary>
        [HttpGet("{id}/report")]
        public async Task<IActionResult> GetReport([FromRoute] string id)
        {
            var task = await _mongoDbContext.Collection<Mongo.Entities.Task>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();
            if (task == null)
            {
                return NotFound();
            }

            var tasks = await _mongoDbContext.Collection<Mongo.Entities.Task>().Find(n => n.SceneId == task.SceneId).ToListAsync();

            var baseline = tasks.SingleOrDefault(n => n.IsBaseline);

            var previous = tasks.Where(n => n.CreationTime < task.CreationTime).OrderByDescending(n => n.CreationTime).FirstOrDefault();

            var report = new TaskReport
            {
                Items = GenerateReport(baseline, previous, task)
            };

            return Ok(report);
        }

        /// <summary>
        /// Create task
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string sceneId)
        {
            var scene = await _mongoDbContext.Collection<Scene>()
                                             .Find(n => n.Id == new ObjectId(sceneId))
                                             .SingleOrDefaultAsync();

            if (scene == null)
            {
                return NotFound();
            }

            var task = new Mongo.Entities.Task
            {
                SceneId = scene.Id,
                Url = scene.Url,
                Method = scene.Method,
                Thread = scene.Thread,
                Connection = scene.Connection,
                Duration = scene.Duration,
                Unit = scene.Unit,
                Status = Mongo.Entities.TaskStatus.Queue,
                CreationTime = DateTime.UtcNow
            };

            await _mongoDbContext.Collection<Mongo.Entities.Task>().InsertOneAsync(task);

            return CreatedAtRoute("GetTask", new { id = task.Id.ToString() }, task);
        }

        [HttpPatch("{id}/baseline")]
        public async Task<IActionResult> SetAsBaseline([FromRoute] string id)
        {
            var task = await _mongoDbContext.Collection<Mongo.Entities.Task>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();
            if (task == null)
            {
                return NotFound();
            }

            if (task.Status != Mongo.Entities.TaskStatus.Done)
            {
                return BadRequest("This task is not finished!");
            }

            var baseline = await _mongoDbContext.Collection<Mongo.Entities.Task>().Find(n => n.SceneId == task.SceneId && n.IsBaseline).SingleOrDefaultAsync();

            if (baseline != null)
            {
                if (baseline.Id == new ObjectId(id))
                {
                    return Ok();
                }
                else
                {
                    var filter = Builders<Mongo.Entities.Task>.Filter.Where(a => a.Id == baseline.Id);
                    var update = Builders<Mongo.Entities.Task>.Update.Set(n => n.IsBaseline, false);
                    await _mongoDbContext.Collection<Mongo.Entities.Task>().FindOneAndUpdateAsync(filter, update);
                }
            }
            else
            {
                var filter = Builders<Mongo.Entities.Task>.Filter.Where(a => a.Id == new ObjectId(id));
                var update = Builders<Mongo.Entities.Task>.Update.Set(n => n.IsBaseline, true);
                await _mongoDbContext.Collection<Mongo.Entities.Task>().FindOneAndUpdateAsync(filter, update);
            }

            return Ok();
        }

        private List<TaskReportItem> GenerateReport(Mongo.Entities.Task baseline, Mongo.Entities.Task previous, Mongo.Entities.Task current)
        {
            var items = new List<TaskReportItem>();

            var item = new TaskReportItem
            {
                Name = "Thread",
                Baseline = baseline?.Thread,
                Previous = previous?.Thread,
                Current = current?.Thread
            };

            items.Add(item);

            item = new TaskReportItem
            {
                Name = "Connection",
                Baseline = baseline?.Connection,
                Previous = previous?.Connection,
                Current = current?.Connection
            };

            items.Add(item);

            item = new TaskReportItem
            {
                Name = "Duration - second",
                Baseline = baseline?.Duration,
                Previous = previous?.Duration,
                Current = current?.Duration
            };

            if (baseline?.Unit == "m")
            {
                item.Baseline = baseline?.Duration * 60;
            }
            else if (baseline?.Unit == "h")
            {
                item.Baseline = baseline?.Duration * 60 * 60;
            }

            if (previous?.Unit == "m")
            {
                item.Previous = previous?.Duration * 60;
            }
            else if (previous?.Unit == "h")
            {
                item.Previous = previous?.Duration * 60 * 60;
            }

            if (current?.Unit == "m")
            {
                item.Current = current?.Duration * 60;
            }
            else if (current?.Unit == "h")
            {
                item.Current = current?.Duration * 60 * 60;
            }

            items.Add(item);

            item = new TaskReportItem
            {
                Name = "Qps",
                Baseline = baseline?.Result?.Qps,
                Previous = previous?.Result?.Qps,
                Current = current?.Result?.Qps
            };

            items.Add(item);

            item = new TaskReportItem
            {
                Name = "Latency 50% - ms",
                Baseline = baseline?.Result?.LatencyP50,
                Previous = previous?.Result?.LatencyP50,
                Current = current?.Result?.LatencyP50
            };

            items.Add(item);

            item = new TaskReportItem
            {
                Name = "Latency 75% - ms",
                Baseline = baseline?.Result?.LatencyP75,
                Previous = previous?.Result?.LatencyP75,
                Current = current?.Result?.LatencyP75
            };

            items.Add(item);

            item = new TaskReportItem
            {
                Name = "Latency 90% - ms",
                Baseline = baseline?.Result?.LatencyP90,
                Previous = previous?.Result?.LatencyP90,
                Current = current?.Result?.LatencyP90
            };

            items.Add(item);

            item = new TaskReportItem
            {
                Name = "Latency 99% - ms",
                Baseline = baseline?.Result?.LatencyP99,
                Previous = previous?.Result?.LatencyP99,
                Current = current?.Result?.LatencyP99
            };

            items.Add(item);

            item = new TaskReportItem
            {
                Name = "Latency Avg - ms",
                Baseline = baseline?.Result?.LatencyAvg,
                Previous = previous?.Result?.LatencyAvg,
                Current = current?.Result?.LatencyAvg
            };

            items.Add(item);

            item = new TaskReportItem
            {
                Name = "Latency Std - ms",
                Baseline = baseline?.Result?.LatencyStd,
                Previous = previous?.Result?.LatencyStd,
                Current = current?.Result?.LatencyStd
            };

            items.Add(item);

            item = new TaskReportItem
            {
                Name = "Latency Max - ms",
                Baseline = baseline?.Result?.LatencyMax,
                Previous = previous?.Result?.LatencyMax,
                Current = current?.Result?.LatencyMax
            };

            items.Add(item);

            return items;
        }
    }
}
