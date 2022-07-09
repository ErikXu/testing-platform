﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Mongo;
using WebApi.Mongo.Entities;

namespace WebApi.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;
        private IMemoryCache _cache;

        public SchedulesController(MongoDbContext mongoDbContext, IMemoryCache cache)
        {
            _mongoDbContext = mongoDbContext;
            _cache = cache;
        }

        /// <summary>
        /// Get schedule list
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var list = await _mongoDbContext.Collection<Schedule>()
                                            .Find(new BsonDocument())
                                            .Sort(Builders<Schedule>.Sort.Descending(n => n.CreationTime))
                                            .ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Get schedule by id
        /// </summary>
        [HttpGet("{id}", Name = "GetSchedule")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var schedule = await _mongoDbContext.Collection<Schedule>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();
            if (schedule == null)
            {
                return NotFound();
            }

            return Ok(schedule);
        }

        /// <summary>
        /// Create schedule
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ScheduleCreateForm form)
        {
            var sceneId = new ObjectId(form.SceneId);
            var sceneName = string.Empty;

            switch (form.TestType)
            {
                case TestType.Stress:
                    var stressScene = await _mongoDbContext.Collection<StressScene>()
                                             .Find(n => n.Id == sceneId)
                                             .SingleOrDefaultAsync();

                    if (stressScene == null)
                    {
                        return NotFound();
                    }

                    sceneName = stressScene.Name;
                    break;
                case TestType.Api:
                    var apiScene = await _mongoDbContext.Collection<ApiScene>()
                                             .Find(n => n.Id == sceneId)
                                             .SingleOrDefaultAsync();
                    if (apiScene == null)
                    {
                        return NotFound();
                    }
                    sceneName = apiScene.Name;

                    break;
                default:
                    return BadRequest("Unsupport test type!");
            }

            var schedule = new Schedule
            {
                SceneId = sceneId,
                SceneName = sceneName,
                TestType = form.TestType,
                Cron = form.Cron,
                Description = form.Description,
                IsDisabled = false,
                CreationTime = DateTime.UtcNow
            };

            await _mongoDbContext.Collection<Schedule>().InsertOneAsync(schedule);

            Enqueue(schedule.Id.ToString());

            return CreatedAtRoute("GetSchedule", new { id = schedule.Id.ToString() }, schedule);
        }

        private void Enqueue(string scheduleId)
        {
            var queue = _cache.Get<Queue<string>>(Program.ScheduleQueueKey) ?? new Queue<string>();

            queue.Enqueue(scheduleId);
            _cache.Set(Program.ScheduleQueueKey, queue);
        }
    }
}
