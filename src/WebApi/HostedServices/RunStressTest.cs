using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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

           StartHtop();

            try
            {
                var script = GenerateScript(task);

                task.Device = new Mongo.Entities.Device
                {
                    TotalMem = GetTotalMem(),
                    AvailableMem = GetAvailableMem()
                };

                var command = $"wrk -t {task.Thread} -c {task.Connection} -s {script} -d {task.Duration}{task.Unit} --latency {task.Url}";
                task.Command = command;

                var (code, message) = ExecuteCommand(command);

                if (code != 0)
                {
                    task.Status = Mongo.Entities.TaskStatus.Error;
                    task.Message = "[Error]" + message;
                    _logger.LogError(message);
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
                _logger.LogError(ex.Message);
            }

            KillHtop();

            task.EndRunningTime = DateTime.UtcNow;
            _mongoDbContext.Collection<Mongo.Entities.Task>().FindOneAndReplace(n => n.Id == task.Id, task);
        }

        private void StartHtop()
        {
            try
            {
                var (code, message) = ExecuteBackgroundCommand("shellinaboxd -t -b -p 8080 --no-beep -s '/:root:root:/:htop -d 10'");
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
                var (code, message) = ExecuteCommand("pgrep -f shellinaboxd -o");
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

                var processId = message;
                _logger.LogInformation($"Process Id of [shellinaboxd] is {processId}!");

                (code, message) = ExecuteCommand($"kill -9 {processId}");
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

        private int GetTotalMem()
        {
            var result = GetDevice("cat /proc/meminfo | grep 'MemTotal' | awk -F':' '{print $2}'");

            if (result == null)
            {
                return 0;
            }

            var value = result.ToLower().Replace("kb", string.Empty).Trim();

            return int.Parse(value) / 1024;
        }

        private int GetAvailableMem()
        {
            var result = GetDevice("cat /proc/meminfo | grep 'MemFree' | awk -F':' '{print $2}'");

            if (result == null)
            {
                return 0;
            }

            var value = result.ToLower().Replace("kb", string.Empty).Trim();

            return int.Parse(value) / 1024;
        }

        private string GetDevice(string command)
        {
            try
            {
                var (code, message) = ExecuteCommand(command);

                if (code == 0)
                {
                    return message;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private string GenerateScript(Mongo.Entities.Task task)
        {
            var script = new StringBuilder();
            script.AppendLine($"wrk.method = \"{task.Method}\"");
            script.AppendLine("wrk.headers[\"Content-Type\"] = \"application/json\"");

            if (task.Method == Program.MethodPost || task.Method == Program.MethodPut || task.Method == Program.MethodPatch)
            {
                script.AppendLine($"wrk.body = \"{task.Body}\"");
            }

            task.Script = script.ToString();

            var scriptPath = Path.Combine(Program.TempFolder, $"{Guid.NewGuid()}.lua");

            File.WriteAllText(scriptPath, task.Script);

            return scriptPath;
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

        private static (int, string) ExecuteBackgroundCommand(string command)
        {
            var escapedArgs = command.Replace("\"", "\\\"");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/sh",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = false,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            var message = string.Empty;
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
