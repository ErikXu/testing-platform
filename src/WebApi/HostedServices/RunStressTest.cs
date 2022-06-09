using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Mongo;
using WebApi.Services;

namespace WebApi.HostedServices
{
    public class RunStressTest : IHostedService
    {
        private Timer _timer;

        private readonly ILogger<RunStressTest> _logger;
        private readonly MongoDbContext _mongoDbContext;
        private readonly IParseService _parseService;

        public RunStressTest(ILogger<RunStressTest> logger, MongoDbContext mongoDbContext, IParseService parseService)
        {
            _logger = logger;
            _mongoDbContext = mongoDbContext;
            _parseService = parseService;
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

            var task = _mongoDbContext.Collection<Mongo.Entities.Task>()
                                      .Find(n => n.Status == Mongo.Entities.TaskStatus.Queue).SortBy(n => n.CreationTime).FirstOrDefault();

            if (task == null)
            {
                return;
            }

            task.Status = Mongo.Entities.TaskStatus.Running;
            task.StartRunningTime = DateTime.UtcNow;

            _mongoDbContext.Collection<Mongo.Entities.Task>().FindOneAndReplace(n => n.Id == task.Id, task);

            var command = $"wrk -t {task.Thread} -c {task.Connection} -d {task.Duration}{task.Unit} --latency {task.Url}";
            task.Command = command;

            try
            {
                var (code, message) = ExecuteCommand(command);

                if (code != 0)
                {
                    task.Status = Mongo.Entities.TaskStatus.Error;
                    task.Message = "[Error]" + message;
                }
                else
                {
                    var result = _parseService.ParseStressTestResult(message);
                    task.Status = Mongo.Entities.TaskStatus.Done;
                    task.Result = result;
                }
            }
            catch (Exception ex)
            {
                task.Status = Mongo.Entities.TaskStatus.Error;
                task.Message = "[Exception]" + ex.Message;
            }

            task.EndRunningTime = DateTime.UtcNow;
            _mongoDbContext.Collection<Mongo.Entities.Task>().FindOneAndReplace(n => n.Id == task.Id, task);
        }

        private static (int, string) ExecuteCommand(string command)
        {
            var escapedArgs = command.Replace("\"", "\\\"");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/sh",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            var message = process.StandardOutput.ReadToEnd();
            if (process.ExitCode != 0)
            {
                message = process.StandardError.ReadToEnd();
            }

            return (process.ExitCode, message);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Run stress test hosted service] is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
