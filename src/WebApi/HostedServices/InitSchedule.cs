using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Mongo;
using WebApi.Mongo.Entities;
using WebApi.Services;

namespace WebApi.HostedServices
{
    public class InitSchedule : IHostedService
    {
        private readonly ILogger<InitSchedule> _logger;
        private readonly MongoDbContext _mongoDbContext;

        public InitSchedule(ILogger<InitSchedule> logger, MongoDbContext mongoDbContext)
        {
            _logger = logger;
            _mongoDbContext = mongoDbContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Init schedule hosted service] is running...");
            var schedules = _mongoDbContext.Collection<Schedule>().AsQueryable().Where(n => !n.IsDisabled).ToList();

            if (schedules.Count == 0)
            {
                return Task.CompletedTask;
            }

            var baseCrons = File.ReadAllText("/var/spool/cron/crontabs/root.bak");

            var crons = new StringBuilder();
            crons.Append(baseCrons);

            foreach (var schedule in schedules)
            {
                crons.AppendLine($"*       *       *       *       *       curl http://localhost/{schedule.SceneId}");
            }

            File.Delete("/var/spool/cron/crontabs/root");
            File.WriteAllText("/var/spool/cron/crontabs/root", crons.ToString());

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Init schedule hosted service] is stopping.");
            return Task.CompletedTask;
        }
    }
}
