﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Agent.HostedServices
{
    public class StartShellinaboxd : IHostedService
    {
        private readonly ILogger<StartShellinaboxd> _logger;

        public StartShellinaboxd(ILogger<StartShellinaboxd> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Start shellinaboxd hosted service] is running...");

            try
            {
                var (code, message) = ExecuteBackgroundCommand("shellinaboxd -t -b -p 5002 --no-beep -s '/:nobody:nogroup:/:htop -d 10'");
                if (code != 0)
                {
                    _logger.LogError(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return Task.CompletedTask;
        }

        public (int, string) ExecuteBackgroundCommand(string command)
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
            _logger.LogInformation("[Start shellinaboxd hosted service] is stopping.");
            return Task.CompletedTask;
        }
    }
}
