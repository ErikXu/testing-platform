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
        /// Get stress scene method list
        /// </summary>
        [HttpGet("method")]
        public IActionResult MethodList()
        {
            var list = _methodDic.Select(n => new { Id = n.Key, Text = n.Value }).ToList();
            return Ok(list);
        }

        /// <summary>
        /// Get stress scene unit list
        /// </summary>
        [HttpGet("unit")]
        public IActionResult UnitList()
        {
            var list = _uintDic.Select(n => new { Id = n.Key, Text = n.Value }).ToList();
            return Ok(list);
        }

        /// <summary>
        /// Get stress scene content-type list
        /// </summary>
        [HttpGet("content-type")]
        public IActionResult ContentTypeList()
        {
            var list = _contentTypeDic.Select(n => new { Id = n.Key, Text = n.Value }).ToList();
            return Ok(list);
        }

        /// <summary>
        /// Get stress scene list
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
        /// Get stress scene options
        /// </summary>
        [HttpGet("options")]
        public async Task<IActionResult> Options()
        {
            var list = await _mongoDbContext.Collection<StressScene>()
                                            .Find(new BsonDocument())
                                            .Sort(Builders<StressScene>.Sort.Ascending(n => n.CreationTime))
                                            .ToListAsync();

            var options = list.Select(n => new Option { Id = n.Id.ToString(), Name = n.Name }).ToList();
            return Ok(options);
        }

        /// <summary>
        /// Get stress scene by id
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
        /// Get stress tasks of scene
        /// </summary>
        [HttpGet("{id}/tasks")]
        public async Task<IActionResult> GetTasksByScene([FromRoute] string id)
        {
            var list = await _mongoDbContext.Collection<StressTask>()
                                            .Find(n => n.SceneId == new ObjectId(id))
                                            .Sort(Builders<StressTask>.Sort.Descending(n => n.CreationTime))
                                            .ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Get stress agents of scene
        /// </summary>
        [HttpGet("{id}/agents")]
        public async Task<IActionResult> GetAgentsByScene([FromRoute] string id)
        {
            var mappings = await _mongoDbContext.Collection<SceneAgentMap>()
                                            .Find(n => n.SceneId == new ObjectId(id))
                                            .ToListAsync();

            var agentIds = mappings.Select(m => m.AgentId).ToList();

            var agents = _mongoDbContext.Collection<Agent>().AsQueryable().Where(n => agentIds.Contains(n.Id)).ToList();

            var now = DateTime.UtcNow;

            foreach (var agent in agents)
            {
                agent.IsActive = (now - agent.LastHeartbeat).TotalSeconds < 30;
            }

            return Ok(agents);
        }

        /// <summary>
        /// Create stress scene
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StressSceneCreateForm form)
        {
            var scene = _mapper.Map<StressScene>(form);
            scene.CreationTime = DateTime.UtcNow;
            await _mongoDbContext.Collection<StressScene>().InsertOneAsync(scene);

            return CreatedAtRoute("GetScene", new { id = scene.Id.ToString() }, scene);
        }

        /// <summary>
        /// Add agent to scene
        /// </summary>
        [HttpPost("{id}/agents")]
        public async Task<IActionResult> AddAgent([FromRoute] string id, [FromBody] AddAgentForm form)
        {
            var scene = await _mongoDbContext.Collection<StressScene>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();
            if (scene == null)
            {
                return NotFound();
            }

            var agent = await _mongoDbContext.Collection<Agent>().Find(n => n.Id == new ObjectId(form.AgentId)).SingleOrDefaultAsync();
            if (agent == null)
            {
                return NotFound();
            }

            var mapping = new SceneAgentMap
            {
                SceneId = scene.Id,
                AgentId = agent.Id
            };

            await _mongoDbContext.Collection<SceneAgentMap>().InsertOneAsync(mapping);

            return Ok();
        }

        /// <summary>
        /// Remove agent from scene
        /// </summary>
        [HttpDelete("{id}/agents")]
        public async Task<IActionResult> RemoveAgent([FromRoute] string id, [FromQuery] string agentId)
        {
            var removeFilter = Builders<SceneAgentMap>.Filter.Where(n => n.SceneId == new ObjectId(id) && n.AgentId == new ObjectId(agentId));

            await _mongoDbContext.Collection<SceneAgentMap>().DeleteManyAsync(removeFilter);

            return NoContent();
        }
    }
}
