using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Mongo;

namespace WebApi.HostedServices
{
    public class RunStressTest : IHostedService
    {
        private Timer _timer;

        private readonly ILogger<RunStressTest> _logger;
        private readonly MongoDbContext _mongoDbContext;

        public RunStressTest(ILogger<RunStressTest> logger, MongoDbContext mongoDbContext)
        {
            _logger = logger;
            _mongoDbContext = mongoDbContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Run stress test hosted service] is running...");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var isRunning = _mongoDbContext.Collection<Mongo.Entities.Task>()
                                           .Find(n => n.Status == Mongo.Entities.TaskStatus.Running)
                                           .Any();

            if (isRunning)
            {
                return;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Run stress test hosted service] is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
