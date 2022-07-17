using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IMapper _mapper;

        public ApiScenesController(MongoDbContext mongoDbContext, IMapper mapper)
        {
            _mongoDbContext = mongoDbContext;
            _mapper = mapper;

        }

        /// <summary>
        /// Get api scene list
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
        /// Get api scene options
        /// </summary>
        [HttpGet("options")]
        public async Task<IActionResult> Options()
        {
            var list = await _mongoDbContext.Collection<ApiScene>()
                                            .Find(new BsonDocument())
                                            .Sort(Builders<ApiScene>.Sort.Ascending(n => n.CreationTime))
                                            .ToListAsync();

            var options = list.Select(n => new Option { Id = n.Id.ToString(), Name = n.Name }).ToList();
            return Ok(options);
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

            var detail = _mapper.Map<ApiSceneDetail>(apiScene);

            if (!string.IsNullOrWhiteSpace(apiScene.Collection))
            {
                try
                {
                    var collectionInfo = JsonConvert.DeserializeObject<CollectionInfo>(apiScene.Collection);
                    if (collectionInfo.Item == null)
                    {
                        detail.CollectionItems = new List<CollectionItem>();
                        detail.IsCollectionInvalid = true;
                    }
                    else
                    {
                        detail.CollectionItems = collectionInfo.Item;
                    }
                }
                catch
                {
                    detail.IsCollectionInvalid = true;
                }
            }

            if (!string.IsNullOrWhiteSpace(apiScene.Environment))
            {
                try
                {
                    var environmentInfo = JsonConvert.DeserializeObject<EnvironmentInfo>(apiScene.Environment);
                    if (environmentInfo.Name == null || environmentInfo.Values == null)
                    {
                        detail.EnvironmentInfo = new EnvironmentInfo();
                        detail.IsEnvironmentInvalid = true;
                    }
                    else
                    {
                        detail.EnvironmentInfo = environmentInfo;
                    }
                }
                catch
                {
                    detail.IsEnvironmentInvalid = true;
                }
            }

            return Ok(detail);
        }

        /// <summary>
        /// Get api tasks of scene
        /// </summary>
        [HttpGet("{id}/tasks")]
        public async Task<IActionResult> ListByScene([FromRoute] string id)
        {
            var list = await _mongoDbContext.Collection<ApiTask>()
                                            .Find(n => n.SceneId == new ObjectId(id))
                                            .Sort(Builders<ApiTask>.Sort.Descending(n => n.CreationTime))
                                            .ToListAsync();
            return Ok(list);
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

        /// <summary>
        /// Upload collection
        /// </summary>
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

        /// <summary>
        /// Upload environment
        /// </summary>
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
