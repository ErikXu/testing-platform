using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using WebApi.Mongo;
using WebApi.Mongo.Entities;

namespace WebApi.Services
{
    public interface IApiTaskService
    {
        public Task<IActionResult> CreateApiTask(string sceneId, ApiTaskFrom from, string caller = null);
    }

    public class ApiTaskService : ControllerBase, IApiTaskService
    {
        private readonly MongoDbContext _mongoDbContext;

        public ApiTaskService(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        public async Task<IActionResult> CreateApiTask(string sceneId, ApiTaskFrom from, string caller = null)
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
                From = from,
                Caller = caller,
                CreationTime = DateTime.UtcNow
            };

            await _mongoDbContext.Collection<ApiTask>().InsertOneAsync(apiTask);

            return CreatedAtRoute("GetApiTask", new { id = apiTask.Id.ToString() }, apiTask);
        }
    }
}
