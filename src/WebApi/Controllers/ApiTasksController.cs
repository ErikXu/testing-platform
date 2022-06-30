using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
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
                CreationTime = DateTime.UtcNow
            };

            await _mongoDbContext.Collection<ApiTask>().InsertOneAsync(apiTask);

            return CreatedAtRoute("GetApiTask", new { id = apiTask.Id.ToString() }, apiTask);
        }

    }
}
