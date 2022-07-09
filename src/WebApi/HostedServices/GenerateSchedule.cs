using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Mongo;
using WebApi.Mongo.Entities;
using WebApi.Services;

namespace WebApi.HostedServices
{
    public class GenerateSchedule : IHostedService
    {
        private Timer _timer;

        private readonly ILogger<GenerateSchedule> _logger;
        private readonly MongoDbContext _mongoDbContext;
        private IMemoryCache _cache;
        private readonly ICommandService _commandService;

        public GenerateSchedule(ILogger<GenerateSchedule> logger,
                                MongoDbContext mongoDbContext,
                                IMemoryCache cache,
                                ICommandService commandService)
        {
            _logger = logger;
            _mongoDbContext = mongoDbContext;
            _cache = cache;
            _commandService = commandService;
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

            var schedules = _mongoDbContext.Collection<Schedule>().AsQueryable().Where(n => !n.IsDisabled).ToList();

            try
            {
                var baseCrons = File.ReadAllText("/var/spool/cron/crontabs/root.bak");

                var crons = new StringBuilder();
                crons.Append(baseCrons);

                if (schedules.Count > 0)
                {
                    foreach (var schedule in schedules)
                    {
                        if (schedule.TestType == TestType.Api)
                        {
                            crons.AppendLine($"{schedule.Cron} curl http://localhost/api/schedules/api-test?sceneId={schedule.SceneId}");
                        }

                        if (schedule.TestType == TestType.Stress)
                        {
                            crons.AppendLine($"{schedule.Cron} curl http://localhost/api/schedules/stress-test?sceneId={schedule.SceneId}");
                        }
                    }
                }

                File.Delete("/var/spool/cron/crontabs/root");
                File.WriteAllText("/var/spool/cron/crontabs/root", crons.ToString());

                _commandService.ExecuteCommand("crontab -r");

                queue.Dequeue();
                _cache.Set(Program.ScheduleQueueKey, queue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
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
