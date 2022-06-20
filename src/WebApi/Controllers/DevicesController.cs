using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using WebApi.Mongo;
using WebApi.Mongo.Entities;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly IDeviceService _deviceService;

        public DevicesController(MongoDbContext mongoDbContext, IDeviceService deviceService)
        {
            _mongoDbContext = mongoDbContext;
            _deviceService = deviceService;
        }

        /// <summary>
        /// Get device info
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var device = await _mongoDbContext.Collection<Device>().Find(new BsonDocument()).SingleOrDefaultAsync();

            if (device == null)
            {
                device = new Device
                {
                    TotalMem = _deviceService.GetTotalMem(),
                    AvailableMem = _deviceService.GetAvailableMem(),
                    CpuCores = _deviceService.GetCpuCores(),
                    CpuModel = _deviceService.GetCpuModel(),
                    CpuFrequency = _deviceService.GetCpuFrequency(),
                    CpuCacheSize = _deviceService.GetCpuCpuCacheSize()
                };

                await _mongoDbContext.Collection<Device>().InsertOneAsync(device);
            }

            return Ok(device);
        }

        /// <summary>
        /// Refresh device info
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Refresh()
        {
            await _mongoDbContext.Collection<Device>().DeleteManyAsync(new BsonDocument());

            var device = new Device
            {
                TotalMem = _deviceService.GetTotalMem(),
                AvailableMem = _deviceService.GetAvailableMem(),
                CpuCores = _deviceService.GetCpuCores(),
                CpuModel = _deviceService.GetCpuModel(),
                CpuFrequency = _deviceService.GetCpuFrequency(),
                CpuCacheSize = _deviceService.GetCpuCpuCacheSize()
            };

            await _mongoDbContext.Collection<Device>().InsertOneAsync(device);

            return Ok(device);
        }
    }
}
