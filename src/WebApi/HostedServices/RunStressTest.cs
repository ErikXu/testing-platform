using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Mongo;
using WebApi.Services;
using WebApi.Mongo.Entities;

namespace WebApi.HostedServices
{
    public class RunStressTest : IHostedService
    {
        private Timer _timer;

        private readonly ILogger<RunStressTest> _logger;
        private readonly MongoDbContext _mongoDbContext;
        private readonly IParseService _parseService;
        private readonly ICommandService _commandService;
        private readonly IDeviceService _deviceService;

        public RunStressTest(ILogger<RunStressTest> logger, 
                             MongoDbContext mongoDbContext, 
                             IParseService parseService,
                             ICommandService commandService,
                             IDeviceService deviceService)
        {
            _logger = logger;
            _mongoDbContext = mongoDbContext;
            _parseService = parseService;
            _commandService = commandService;
            _deviceService = deviceService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Run stress test hosted service] is running...");

            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var isRunning = _mongoDbContext.Collection<StressTask>()
                                           .Find(n => n.Status == StressTaskStatus.Running)
                                           .Any();

            if (isRunning)
            {
                return;
            }

            var task = _mongoDbContext.Collection<StressTask>()
                                      .Find(n => n.Status == StressTaskStatus.Queue).SortBy(n => n.CreationTime).FirstOrDefault();

            if (task == null)
            {
                return;
            }

            task.Status = StressTaskStatus.Running;
            task.StartRunningTime = DateTime.UtcNow;

            _mongoDbContext.Collection<StressTask>().FindOneAndReplace(n => n.Id == task.Id, task);

            StartHtop();

            try
            {
                var script = GenerateScript(task);

                task.Device = new Device
                {
                    TotalMem = _deviceService.GetTotalMem(),
                    AvailableMem = _deviceService.GetAvailableMem()
                };

                var command = $"wrk -t {task.Thread} -c {task.Connection} -s {script} -d {task.Duration}{task.Unit} --latency {task.Url}";
                task.Command = command;

                var (code, message) = _commandService.ExecuteCommand(command);

                if (code != 0)
                {
                    task.Status = StressTaskStatus.Error;
                    task.Message = "[Error]" + message;
                    _logger.LogError(message);
                }
                else
                {
                    var result = _parseService.ParseStressTestResult(message);
                    task.Status = StressTaskStatus.Done;
                    task.Result = result;
                }
            }
            catch (Exception ex)
            {
                task.Status = StressTaskStatus.Error;
                task.Message = "[Exception]" + ex.Message;
                _logger.LogError(ex.Message);
            }

            KillHtop();

            task.EndRunningTime = DateTime.UtcNow;
            _mongoDbContext.Collection<StressTask>().FindOneAndReplace(n => n.Id == task.Id, task);
        }

        private void StartHtop()
        {
            try
            {
                var (code, message) = _commandService.ExecuteBackgroundCommand("shellinaboxd -t -b -p 8080 --no-beep -s '/:nobody:nogroup:/:htop -d 10'");
                if (code != 0)
                {
                    _logger.LogError(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private void KillHtop()
        {
            try
            {
                var (code, message) = _commandService.ExecuteCommand("pgrep -f shellinaboxd -o");
                if (code != 0)
                {
                    _logger.LogError(message);
                    return;
                }

                if (string.IsNullOrWhiteSpace(message))
                {
                    _logger.LogError("Process [shellinaboxd] is not found!");
                    return;
                }

                var processId = message.Trim();
                _logger.LogInformation($"Process Id of [shellinaboxd] is {processId}!");

                (code, message) = _commandService.ExecuteCommand($"kill -9 {processId}");
                if (code != 0)
                {
                    _logger.LogError(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private string GenerateScript(StressTask task)
        {
            var script = new StringBuilder();
            script.AppendLine($"wrk.method = '{task.Method}'");
            script.AppendLine($"wrk.headers['Content-Type'] = '{task.ContentType}'");

            if (task.Method == Program.MethodPost || task.Method == Program.MethodPut || task.Method == Program.MethodPatch)
            {
                script.AppendLine($"wrk.body = '{task.Body ?? string.Empty}'");
            }

            task.Script = script.ToString();

            var scriptPath = Path.Combine(Program.TempFolder, $"{Guid.NewGuid()}.lua");

            File.WriteAllText(scriptPath, task.Script);

            return scriptPath;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Run stress test hosted service] is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
