using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using WebApi.Mongo;
using WebApi.Mongo.Entities;

namespace WebApi.Services
{
    public interface IStressTaskService
    {
        public Task<IActionResult> CreateStressTask(string sceneId, StressTaskFrom from);
    }

    public class StressTaskService : ControllerBase, IStressTaskService
    {
        private readonly MongoDbContext _mongoDbContext;

        public StressTaskService(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        public async Task<IActionResult> CreateStressTask(string sceneId, StressTaskFrom from)
        {
            var stressScene = await _mongoDbContext.Collection<StressScene>()
                                                         .Find(n => n.Id == new ObjectId(sceneId))
                                                         .SingleOrDefaultAsync();

            if (stressScene == null)
            {
                return NotFound();
            }

            var task = new StressTask
            {
                SceneId = stressScene.Id,
                SceneName = stressScene.Name,
                Url = stressScene.Url,
                Method = stressScene.Method,
                Thread = stressScene.Thread,
                Connection = stressScene.Connection,
                Duration = stressScene.Duration,
                Unit = stressScene.Unit,
                Status = StressTaskStatus.Queue,
                From = from,
                CreationTime = DateTime.UtcNow
            };

            await _mongoDbContext.Collection<StressTask>().InsertOneAsync(task);

            return CreatedAtRoute("GetStressTask", new { id = task.Id.ToString() }, task);
        }
    }
}
