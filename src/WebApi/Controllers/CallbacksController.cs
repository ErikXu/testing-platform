using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using WebApi.Mongo;
using WebApi.Mongo.Entities;

namespace WebApi.Controllers
{
    [Route("api/callbacks")]
    [ApiController]
    public class CallbacksController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;

        public CallbacksController(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        /// <summary>
        /// Get callback setting
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var setting = await _mongoDbContext.Collection<CallbackSetting>().Find(new BsonDocument()).SingleOrDefaultAsync();

            if (setting == null)
            {
                setting = new CallbackSetting();
                await _mongoDbContext.Collection<CallbackSetting>().InsertOneAsync(setting);
            }

            return Ok(setting);
        }

        /// <summary>
        /// Api test callback
        /// </summary>
        [HttpGet("stress-test")]
        public async Task<IActionResult> CallbackStressTest([FromQuery] string sceneId, [FromQuery] string caller)
        {
            var setting = await _mongoDbContext.Collection<CallbackSetting>().Find(new BsonDocument()).SingleOrDefaultAsync();

            if (setting == null)
            {
                setting = new CallbackSetting();
                await _mongoDbContext.Collection<CallbackSetting>().InsertOneAsync(setting);
            }

            if (!setting.IsStressTestEnabled)
            {
                return BadRequest("Stress test callback is not enabled!");
            }

            var scene = await _mongoDbContext.Collection<StressScene>()
                                             .Find(n => n.Id == new ObjectId(sceneId))
                                             .SingleOrDefaultAsync();

            if (scene == null)
            {
                return NotFound();
            }

            var task = new StressTask
            {
                SceneId = scene.Id,
                SceneName = scene.Name,
                Url = scene.Url,
                Method = scene.Method,
                Thread = scene.Thread,
                Connection = scene.Connection,
                Duration = scene.Duration,
                Unit = scene.Unit,
                Status = StressTaskStatus.Queue,
                From = StressTaskFrom.Callback,
                Caller = caller,
                CreationTime = DateTime.UtcNow
            };

            await _mongoDbContext.Collection<StressTask>().InsertOneAsync(task);

            return CreatedAtRoute("GetStressTask", new { id = task.Id.ToString() }, task);
        }

        /// <summary>
        /// Switch stress test callback setting
        /// </summary>
        [HttpPatch("{id}/stress-test")]
        public async Task<IActionResult> SwitchStressTest([FromRoute] string id)
        {
            var setting = await _mongoDbContext.Collection<CallbackSetting>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();

            if (setting == null)
            {
                return NotFound();
            }

            var filter = Builders<CallbackSetting>.Filter.Where(a => a.Id == new ObjectId(id));
            var update = Builders<CallbackSetting>.Update.Set(n => n.IsStressTestEnabled, !setting.IsStressTestEnabled);
            await _mongoDbContext.Collection<CallbackSetting>().FindOneAndUpdateAsync(filter, update);

            return Ok();
        }

        /// <summary>
        /// Api test callback
        /// </summary>
        [HttpGet("api-test")]
        public async Task<IActionResult> CallbackApiTest([FromQuery] string sceneId, [FromQuery] string caller)
        {
            var setting = await _mongoDbContext.Collection<CallbackSetting>().Find(new BsonDocument()).SingleOrDefaultAsync();

            if (setting == null)
            {
                setting = new CallbackSetting();
                await _mongoDbContext.Collection<CallbackSetting>().InsertOneAsync(setting);
            }

            if (!setting.IsApiTestEnabled)
            {
                return BadRequest("Api test callback is not enabled!");
            }

            var apiScene = await _mongoDbContext.Collection<ApiScene>().Find(n => n.Id == new ObjectId(sceneId)).SingleOrDefaultAsync();

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
                From = ApiTaskFrom.Callback,
                Caller = caller,
                CreationTime = DateTime.UtcNow
            };

            await _mongoDbContext.Collection<ApiTask>().InsertOneAsync(apiTask);

            return CreatedAtRoute("GetApiTask", new { id = apiTask.Id.ToString() }, apiTask);
        }

        /// <summary>
        /// Switch api test callback setting
        /// </summary>
        [HttpPatch("{id}/api-test")]
        public async Task<IActionResult> SwitchApiTest([FromRoute] string id)
        {
            var setting = await _mongoDbContext.Collection<CallbackSetting>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();

            if (setting == null)
            {
                return NotFound();
            }

            var filter = Builders<CallbackSetting>.Filter.Where(a => a.Id == new ObjectId(id));
            var update = Builders<CallbackSetting>.Update.Set(n => n.IsApiTestEnabled, !setting.IsApiTestEnabled);
            await _mongoDbContext.Collection<CallbackSetting>().FindOneAndUpdateAsync(filter, update);

            return Ok();
        }
    }
}
