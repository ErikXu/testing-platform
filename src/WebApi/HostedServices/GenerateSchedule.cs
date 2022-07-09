using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Mongo;
using WebApi.Mongo.Entities;

namespace WebApi.HostedServices
{
    public class GenerateSchedule : IHostedService
    {
        private Timer _timer;

        private readonly ILogger<GenerateSchedule> _logger;
        private readonly MongoDbContext _mongoDbContext;
        private IMemoryCache _cache;

        public GenerateSchedule(ILogger<GenerateSchedule> logger, MongoDbContext mongoDbContext, IMemoryCache cache)
        {
            _logger = logger;
            _mongoDbContext = mongoDbContext;
            _cache = cache;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Generate schedule hosted service] is running...");

            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var queue = _cache.Get<Queue<string>>(Program.ScheduleQueueKey) ?? new Queue<string>();
            if (queue.Count == 0)
            {
                return;
            }

            var list = _mongoDbContext.Collection<Schedule>().AsQueryable().Where(n => !n.IsDisabled).ToList();

            if (list.Count > 0)
            {

            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Generate schedule hosted service] is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
