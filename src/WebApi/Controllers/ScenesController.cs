using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Mongo;
using WebApi.Mongo.Entities;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScenesController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly IMapper _mapper;

        public ScenesController(MongoDbContext mongoDbContext, IMapper mapper)
        {
            _mongoDbContext = mongoDbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var list = await _mongoDbContext.Collection<Scene>()
                                            .Find(new BsonDocument())
                                            .Sort(Builders<Scene>.Sort.Ascending(n => n.CreationTime))
                                            .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}", Name = "GetScene")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var scene = await _mongoDbContext.Collection<Scene>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();
            if (scene == null)
            {
                return NotFound();
            }
            return Ok(scene);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SceneCreateForm form)
        {
            var scene = _mapper.Map<Scene>(form);
            scene.CreationTime = DateTime.UtcNow;
            await _mongoDbContext.Collection<Scene>().InsertOneAsync(scene);

            return CreatedAtRoute("GetScene", new { id = scene.Id.ToString() }, scene);
        }
    }
}
