﻿using Microsoft.AspNetCore.Mvc;
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
        /// Get task scene by id
        /// </summary>
        [HttpGet("{id}/scene")]
        public async Task<IActionResult> GetScene([FromRoute] string id)
        {
            var task = await _mongoDbContext.Collection<Mongo.Entities.Task>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();
            if (task == null)
            {
                return NotFound();
            }

            var scene = await _mongoDbContext.Collection<Scene>().Find(n => n.Id == task.SceneId).SingleOrDefaultAsync();
            if (scene == null)
            {
                return NotFound();
            }

            return Ok(scene);
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

            items.Add(GenerateItemOfId(baseline, previous, current));
            items.Add(GenerateItemOfThread(baseline, previous, current));
            items.Add(GenerateItemOfConnection(baseline, previous, current));
            items.Add(GenerateItemOfDuration(baseline, previous, current));
            items.Add(GenerateItemOfQps(baseline, previous, current));
            items.Add(GenerateItemOfLatencyP50(baseline, previous, current));
            items.Add(GenerateItemOfLatencyP75(baseline, previous, current));
            items.Add(GenerateItemOfLatencyP90(baseline, previous, current));
            items.Add(GenerateItemOfLatencyP99(baseline, previous, current));
            items.Add(GenerateItemOfLatencyAvg(baseline, previous, current));
            items.Add(GenerateItemOfLatencyStd(baseline, previous, current));
            items.Add(GenerateItemOfLatencyMax(baseline, previous, current));

            return items;
        }

        private TaskReportItem GenerateItemOfId(Mongo.Entities.Task baseline, Mongo.Entities.Task previous, Mongo.Entities.Task current)
        {
            var item = new TaskReportItem
            {
                Name = "Id",
                Baseline = baseline?.Id.ToString(),
                Previous = previous?.Id.ToString(),
                Current = current?.Id.ToString()
            };

            return item;
        }

        private TaskReportItem GenerateItemOfThread(Mongo.Entities.Task baseline, Mongo.Entities.Task previous, Mongo.Entities.Task current)
        {
            var item = new TaskReportItem
            {
                Name = "Thread",
                Baseline = baseline?.Thread.ToString(),
                Previous = previous?.Thread.ToString(),
                Current = current?.Thread.ToString()
            };

            return item;
        }

        private TaskReportItem GenerateItemOfConnection(Mongo.Entities.Task baseline, Mongo.Entities.Task previous, Mongo.Entities.Task current)
        {
            var item = new TaskReportItem
            {
                Name = "Connection",
                Baseline = baseline?.Connection.ToString(),
                Previous = previous?.Connection.ToString(),
                Current = current?.Connection.ToString()
            };

            return item;
        }

        private TaskReportItem GenerateItemOfDuration(Mongo.Entities.Task baseline, Mongo.Entities.Task previous, Mongo.Entities.Task current)
        {
            var item = new TaskReportItem
            {
                Name = "Duration - second",
                Baseline = baseline?.Duration.ToString(),
                Previous = previous?.Duration.ToString(),
                Current = current?.Duration.ToString()
            };

            if (baseline?.Unit == "m")
            {
                item.Baseline = (baseline?.Duration * 60).ToString();
            }
            else if (baseline?.Unit == "h")
            {
                item.Baseline = (baseline?.Duration * 60 * 60).ToString();
            }

            if (previous?.Unit == "m")
            {
                item.Previous = (previous?.Duration * 60).ToString();
            }
            else if (previous?.Unit == "h")
            {
                item.Previous = (previous?.Duration * 60 * 60).ToString();
            }

            if (current?.Unit == "m")
            {
                item.Current = (current?.Duration * 60).ToString();
            }
            else if (current?.Unit == "h")
            {
                item.Current = (current?.Duration * 60 * 60).ToString();
            }

            return item;
        }

        private TaskReportItem GenerateItemOfQps(Mongo.Entities.Task baseline, Mongo.Entities.Task previous, Mongo.Entities.Task current)
        {
            var item = new TaskReportItem
            {
                Name = "Qps",
                Baseline = baseline?.Result?.Qps.ToString(),
                Previous = previous?.Result?.Qps.ToString(),
                Current = current?.Result?.Qps.ToString(),
                BaselineToCurrent = MoreIsBetter(baseline?.Result?.Qps ?? 0, current?.Result?.Qps ?? 0),
                PreviousToCurrent = MoreIsBetter(previous?.Result?.Qps ?? 0, current?.Result?.Qps ?? 0)
            };

            return item;
        }

        private TaskReportItem GenerateItemOfLatencyP50(Mongo.Entities.Task baseline, Mongo.Entities.Task previous, Mongo.Entities.Task current)
        {
            var item = new TaskReportItem
            {
                Name = "Latency 50% - ms",
                Baseline = baseline?.Result?.LatencyP50.ToString(),
                Previous = previous?.Result?.LatencyP50.ToString(),
                Current = current?.Result?.LatencyP50.ToString(),
                BaselineToCurrent = LessIsBetter(baseline?.Result?.LatencyP50 ?? 0, current?.Result?.LatencyP50 ?? 0),
                PreviousToCurrent = LessIsBetter(previous?.Result?.LatencyP50 ?? 0, current?.Result?.LatencyP50 ?? 0)
            };

            return item;
        }

        private TaskReportItem GenerateItemOfLatencyP75(Mongo.Entities.Task baseline, Mongo.Entities.Task previous, Mongo.Entities.Task current)
        {
            var item = new TaskReportItem
            {
                Name = "Latency 75% - ms",
                Baseline = baseline?.Result?.LatencyP75.ToString(),
                Previous = previous?.Result?.LatencyP75.ToString(),
                Current = current?.Result?.LatencyP75.ToString(),
                BaselineToCurrent = LessIsBetter(baseline?.Result?.LatencyP75 ?? 0, current?.Result?.LatencyP75 ?? 0),
                PreviousToCurrent = LessIsBetter(previous?.Result?.LatencyP75 ?? 0, current?.Result?.LatencyP75 ?? 0)
            };

            return item;
        }

        private TaskReportItem GenerateItemOfLatencyP90(Mongo.Entities.Task baseline, Mongo.Entities.Task previous, Mongo.Entities.Task current)
        {
            var item = new TaskReportItem
            {
                Name = "Latency 90% - ms",
                Baseline = baseline?.Result?.LatencyP90.ToString(),
                Previous = previous?.Result?.LatencyP90.ToString(),
                Current = current?.Result?.LatencyP90.ToString(),
                BaselineToCurrent = LessIsBetter(baseline?.Result?.LatencyP90 ?? 0, current?.Result?.LatencyP90 ?? 0),
                PreviousToCurrent = LessIsBetter(previous?.Result?.LatencyP90 ?? 0, current?.Result?.LatencyP90 ?? 0)
            };

            return item;
        }

        private TaskReportItem GenerateItemOfLatencyP99(Mongo.Entities.Task baseline, Mongo.Entities.Task previous, Mongo.Entities.Task current)
        {
            var item = new TaskReportItem
            {
                Name = "Latency 99% - ms",
                Baseline = baseline?.Result?.LatencyP99.ToString(),
                Previous = previous?.Result?.LatencyP99.ToString(),
                Current = current?.Result?.LatencyP99.ToString(),
                BaselineToCurrent = LessIsBetter(baseline?.Result?.LatencyP99 ?? 0, current?.Result?.LatencyP99 ?? 0),
                PreviousToCurrent = LessIsBetter(previous?.Result?.LatencyP99 ?? 0, current?.Result?.LatencyP99 ?? 0)
            };

            return item;
        }

        private TaskReportItem GenerateItemOfLatencyAvg(Mongo.Entities.Task baseline, Mongo.Entities.Task previous, Mongo.Entities.Task current)
        {
            var item = new TaskReportItem
            {
                Name = "Latency Avg - ms",
                Baseline = baseline?.Result?.LatencyAvg.ToString(),
                Previous = previous?.Result?.LatencyAvg.ToString(),
                Current = current?.Result?.LatencyAvg.ToString(),
                BaselineToCurrent = LessIsBetter(baseline?.Result?.LatencyAvg ?? 0, current?.Result?.LatencyAvg ?? 0),
                PreviousToCurrent = LessIsBetter(previous?.Result?.LatencyAvg ?? 0, current?.Result?.LatencyAvg ?? 0)
            };

            return item;
        }

        private TaskReportItem GenerateItemOfLatencyStd(Mongo.Entities.Task baseline, Mongo.Entities.Task previous, Mongo.Entities.Task current)
        {
            var item = new TaskReportItem
            {
                Name = "Latency Std - ms",
                Baseline = baseline?.Result?.LatencyStd.ToString(),
                Previous = previous?.Result?.LatencyStd.ToString(),
                Current = current?.Result?.LatencyStd.ToString(),
                BaselineToCurrent = LessIsBetter(baseline?.Result?.LatencyStd ?? 0, current?.Result?.LatencyStd ?? 0),
                PreviousToCurrent = LessIsBetter(previous?.Result?.LatencyStd ?? 0, current?.Result?.LatencyStd ?? 0)
            };

            return item;
        }

        private TaskReportItem GenerateItemOfLatencyMax(Mongo.Entities.Task baseline, Mongo.Entities.Task previous, Mongo.Entities.Task current)
        {
            var item = new TaskReportItem
            {
                Name = "Latency Max - ms",
                Baseline = baseline?.Result?.LatencyMax.ToString(),
                Previous = previous?.Result?.LatencyMax.ToString(),
                Current = current?.Result?.LatencyMax.ToString(),
                BaselineToCurrent = LessIsBetter(baseline?.Result?.LatencyMax ?? 0, current?.Result?.LatencyMax ?? 0),
                PreviousToCurrent = LessIsBetter(previous?.Result?.LatencyMax ?? 0, current?.Result?.LatencyMax ?? 0)
            };

            return item;
        }

        private int MoreIsBetter(double from, double to)
        {
            if (from > to)
            {
                return 1;
            }

            if (from < to)
            {
                return -1;
            }

            return 0;
        }

        private int LessIsBetter(double from, double to)
        {
            if (from < to)
            {
                return 1;
            }

            if (from > to)
            {
                return -1;
            }

            return 0;
        }
    }
}
