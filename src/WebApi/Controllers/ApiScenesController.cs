using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.IO;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Mongo;
using WebApi.Mongo.Entities;

namespace WebApi.Controllers
{
    [Route("api/api-scenes")]
    [ApiController]
    public class ApiScenesController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;

        public ApiScenesController(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;

        }
        /// <summary>
        /// Get scene list
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var list = await _mongoDbContext.Collection<ApiScene>()
                                            .Find(new BsonDocument())
                                            .Sort(Builders<ApiScene>.Sort.Ascending(n => n.CreationTime))
                                            .ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Get api scene by id
        /// </summary>
        [HttpGet("{id}", Name = "GetApiScene")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var apiScene = await _mongoDbContext.Collection<ApiScene>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();
            if (apiScene == null)
            {
                return NotFound();
            }
            return Ok(apiScene);
        }

        /// <summary>
        /// Create api scene
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApiSceneCreateForm form)
        {
            var apiScene = new ApiScene
            {
                Name = form.Name,
                Description = form.Description,
                CreationTime = DateTime.UtcNow
            };

            await _mongoDbContext.Collection<ApiScene>().InsertOneAsync(apiScene);

            return CreatedAtRoute("GetApiScene", new { id = apiScene.Id.ToString() }, apiScene);
        }

        [HttpPost("{id}/collection")]
        public async Task<IActionResult> UploadCollection([FromRoute] string id, IFormFile file)
        {
            var apiScene = await _mongoDbContext.Collection<ApiScene>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();
            if (apiScene == null)
            {
                return NotFound();
            }

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            var collection = await reader.ReadToEndAsync();

            apiScene.Collection = collection;
            _mongoDbContext.Collection<ApiScene>().FindOneAndReplace(n => n.Id == apiScene.Id, apiScene);

            return Ok(apiScene);
        }

        [HttpPost("{id}/environment")]
        public async Task<IActionResult> UploadEnvironment([FromRoute] string id, IFormFile file)
        {
            var apiScene = await _mongoDbContext.Collection<ApiScene>().Find(n => n.Id == new ObjectId(id)).SingleOrDefaultAsync();
            if (apiScene == null)
            {
                return NotFound();
            }

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            var environment = await reader.ReadToEndAsync();

            apiScene.Environment = environment;
            _mongoDbContext.Collection<ApiScene>().FindOneAndReplace(n => n.Id == apiScene.Id, apiScene);

            return Ok(apiScene);
        }
    }
}
