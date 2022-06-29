using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Mongo;
using WebApi.Mongo.Entities;

namespace WebApi.HostedServices
{
    public class RunApiTest : IHostedService
    {
        private Timer _timer;

        private readonly ILogger<RunApiTest> _logger;
        private readonly MongoDbContext _mongoDbContext;

        public RunApiTest(ILogger<RunApiTest> logger, MongoDbContext mongoDbContext)
        {
            _logger = logger;
            _mongoDbContext = mongoDbContext;

        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Run api test hosted service] is running...");

            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var isRunning = _mongoDbContext.Collection<ApiTask>()
                               .Find(n => n.Status == ApiTaskStatus.Running)
                               .Any();

            if (isRunning)
            {
                return;
            }

            var apiTask = _mongoDbContext.Collection<ApiTask>()
                                     .Find(n => n.Status == ApiTaskStatus.Queue && n.Collection != null)
                                     .SortBy(n => n.CreationTime)
                                     .FirstOrDefault();

            if (apiTask == null)
            {
                return;
            }

            apiTask.Status = ApiTaskStatus.Running;
            apiTask.StartRunningTime = DateTime.UtcNow;
            _mongoDbContext.Collection<ApiTask>().FindOneAndReplace(n => n.Id == apiTask.Id, apiTask);

            try
            {
                var collectionPath = Path.Combine(Program.TempFolder, $"{Guid.NewGuid()}.json");
                File.WriteAllText(collectionPath, apiTask.Collection);

                var resultPath = Path.Combine(Program.TempFolder, $"{Guid.NewGuid()}.json");

                var command = $"newman run {collectionPath} --suppress-exit-code 1 --reporters json --reporter-json-export {resultPath}";

                if (!string.IsNullOrWhiteSpace(apiTask.Environment))
                {
                    var environmentPath = Path.Combine(Program.TempFolder, $"{Guid.NewGuid()}.json");
                    File.WriteAllText(environmentPath, apiTask.Environment);
                    command = $"newman run {collectionPath} --suppress-exit-code 1 --reporters json --reporter-json-export {resultPath} --environment {environmentPath}";
                }

                apiTask.Command = command;

                var (code, message) = ExecuteCommand(command);

                if (code != 0)
                {
                    apiTask.Status = ApiTaskStatus.Error;
                    apiTask.Message = "[Error]" + message;
                    _logger.LogError(message);
                }
                else
                {
                    var resultJson = File.ReadAllText(resultPath);
                    var result = JsonConvert.DeserializeObject<NewmanResult>(resultJson);
                    apiTask.Status = ApiTaskStatus.Done;
                    apiTask.Result = result;
                }
            }
            catch (Exception ex)
            {
                apiTask.Status = ApiTaskStatus.Error;
                apiTask.Message = "[Exception]" + ex.Message;
                _logger.LogError(ex.Message);
            }

            apiTask.EndRunningTime = DateTime.UtcNow;
            _mongoDbContext.Collection<ApiTask>().FindOneAndReplace(n => n.Id == apiTask.Id, apiTask);
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
            _logger.LogInformation("[Run api test hosted service] is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
