using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using WebApi.Mongo;
using WebApi.Mongo.Entities;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/callbacks")]
    [ApiController]
    public class CallbacksController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly IApiTaskService _apiTaskService;
        private readonly IStressTaskService _stressTaskService;

        public CallbacksController(MongoDbContext mongoDbContext, 
                                   IApiTaskService apiTaskService, 
                                   IStressTaskService stressTaskService)
        {
            _mongoDbContext = mongoDbContext;
            _apiTaskService = apiTaskService;
            _stressTaskService = stressTaskService;
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

            return await _stressTaskService.CreateStressTask(sceneId, StressTaskFrom.Callback);
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

            return await _apiTaskService.CreateApiTask(sceneId, ApiTaskFrom.Callback);
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
