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
    public class AgentsController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;

        public AgentsController(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        /// <summary>
        /// Get agent list
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var agents = await _mongoDbContext.Collection<Agent>()
                                            .Find(new BsonDocument())
                                            .Sort(Builders<Agent>.Sort.Ascending(n => n.CreationTime))
                                            .ToListAsync();

            var now = DateTime.UtcNow;

            foreach (var agent in agents)
            {
                agent.IsActive = (now - agent.LastHeartbeat).TotalSeconds < 30;
            }

            return Ok(agents);
        }

        /// <summary>
        /// Accept heartbeat
        /// </summary>
        /// <returns></returns>
        [HttpGet("heartbeat")]
        public async Task<IActionResult> Heartbeat([FromQuery] string agentAddress)
        {
            var agent = await _mongoDbContext.Collection<Agent>().Find(n => n.AgentAddress == agentAddress).SingleOrDefaultAsync();

            var now = DateTime.UtcNow;

            if (agent == null)
            {
                agent = new Agent
                {
                    AgentAddress = agentAddress,
                    LastHeartbeat = now,
                    CreationTime = now
                };

                await _mongoDbContext.Collection<Agent>().InsertOneAsync(agent);
            }
            else
            {
                agent.LastHeartbeat = now;
                await _mongoDbContext.Collection<Agent>().ReplaceOneAsync(n => n.Id == agent.Id, agent);
            }

            return Ok(agent);
        }
    }
}
