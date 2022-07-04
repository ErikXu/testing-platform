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
                CreationTime = DateTime.UtcNow
            };

            await _mongoDbContext.Collection<ApiTask>().InsertOneAsync(apiTask);

            return CreatedAtRoute("GetApiTask", new { id = apiTask.Id.ToString() }, apiTask);
        }
    }
}
