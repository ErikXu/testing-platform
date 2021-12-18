using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using WebApi.Mongo;
using WebApi.Mongo.Entities;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;

        public TasksController(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }


        [HttpGet]
        public async Task<IActionResult> ListByScene([FromQuery] string sceneId)
        {
            var list = await _mongoDbContext.Collection<Mongo.Entities.Task>()
                                            .Find(n => n.SceneId == new ObjectId(sceneId))
                                            .Sort(Builders<Mongo.Entities.Task>.Sort.Ascending(n => n.CreationTime))
                                            .ToListAsync();
            return Ok(list);
        }

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
    }
}
