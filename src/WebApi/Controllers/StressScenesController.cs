using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Mongo;
using WebApi.Mongo.Entities;

namespace WebApi.Controllers
{
    [Route("api/stress-scenes")]
    [ApiController]
    public class StressScenesController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly IMapper _mapper;

        private readonly Dictionary<string, string> _methodDic = new Dictionary<string, string>
        {
            { Program.MethodGet, Program.MethodGet },
            { Program.MethodPost, Program.MethodPost },
            { Program.MethodPut, Program.MethodPut },
            { Program.MethodPatch, Program.MethodPatch },
            { Program.MethodDelete, Program.MethodDelete }
        };

        private readonly Dictionary<string, string> _uintDic = new Dictionary<string, string>
        {
            { "s", "Second" },
            { "m", "Munite" },
            { "h", "Hour" }
        };

        private readonly Dictionary<string, string> _contentTypeDic = new Dictionary<string, string>
        {
            { "application/json", "application/json" },
            { "application/x-www-form-urlencoded", "application/x-www-form-urlencoded" }
        };

        public StressScenesController(MongoDbContext mongoDbContext, IMapper mapper)
        {
            _mongoDbContext = mongoDbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Get scene method list
        /// </summary>
        [HttpGet("method")]
        public IActionResult MethodList()
        {
            var list = _methodDic.Select(n => new { Id = n.Key, Text = n.Value }).ToList();
            return Ok(list);
        }

        /// <summary>
        /// Get scene unit list
        /// </summary>
        [HttpGet("unit")]
        public IActionResult UnitList()
        {
            var list = _uintDic.Select(n => new { Id = n.Key, Text = n.Value }).ToList();
            return Ok(list);
        }

        /// <summary>
        /// Get scene content-type list
        /// </summary>
        [HttpGet("content-type")]
        public IActionResult ContentTypeList()
        {
            var list = _contentTypeDic.Select(n => new { Id = n.Key, Text = n.Value }).ToList();
            return Ok(list);
        }

        /// <summary>
        /// Get scene list
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var list = await _mongoDbContext.Collection<StressScene>()
                                            .Find(new BsonDocument())
                                            .Sort(Builders<StressScene>.Sort.Ascending(n => n.CreationTime))
                                            .ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Get scene by id
        /// </summary>
        [HttpGet("{id}", Name = "GetScene")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var scene = await _mongoDbContext.Collection<StressScene>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();
            if (scene == null)
            {
                return NotFound();
            }

            return Ok(scene);
        }

        /// <summary>
        /// Get tasks of scene
        /// </summary>
        [HttpGet("{id}/tasks")]
        public async Task<IActionResult> ListByScene([FromRoute] string id)
        {
            var list = await _mongoDbContext.Collection<StressTask>()
                                            .Find(n => n.SceneId == new ObjectId(id))
                                            .Sort(Builders<StressTask>.Sort.Descending(n => n.CreationTime))
                                            .ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Create scene
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StressSceneCreateForm form)
        {
            var scene = _mapper.Map<StressScene>(form);
            scene.CreationTime = DateTime.UtcNow;
            await _mongoDbContext.Collection<StressScene>().InsertOneAsync(scene);

            return CreatedAtRoute("GetScene", new { id = scene.Id.ToString() }, scene);
        }
    }
}
