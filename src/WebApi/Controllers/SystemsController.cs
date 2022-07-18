using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;
using WebApi.Mongo;
using WebApi.Mongo.Entities;

namespace WebApi.Controllers
{
    [Route("api/systems")]
    [ApiController]
    public class SystemsController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;

        public SystemsController(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        /// <summary>
        /// Remove the whole data
        /// </summary>
        [HttpDelete("reset")]
        public async Task<IActionResult> Reset()
        {
            await _mongoDbContext.Collection<StressScene>().DeleteManyAsync(new BsonDocument());
            await _mongoDbContext.Collection<StressTask>().DeleteManyAsync(new BsonDocument());
            await _mongoDbContext.Collection<ApiScene>().DeleteManyAsync(new BsonDocument());
            await _mongoDbContext.Collection<ApiTask>().DeleteManyAsync(new BsonDocument());
            await _mongoDbContext.Collection<Device>().DeleteManyAsync(new BsonDocument());
            await _mongoDbContext.Collection<CallbackSetting>().DeleteManyAsync(new BsonDocument());
            await _mongoDbContext.Collection<Schedule>().DeleteManyAsync(new BsonDocument());
            return NoContent();
        }
    }
}
