using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Agent.HostedServices
{
    public class SendHeartbeat : IHostedService
    {
        private Timer _timer;

        private readonly ILogger<SendHeartbeat> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public SendHeartbeat(ILogger<SendHeartbeat> logger,
                             IHttpClientFactory httpClientFactory,
                             IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Send heartbeat hosted service] is running...");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                var serverDomain = _configuration["ServerDomain"].Trim('/');
                var clientAddress = _configuration["ClientAddress"];

                if (int.TryParse(_configuration["AgentPort"], out var agentPort))
                {
                    agentPort = 5001;
                }

                if (int.TryParse(_configuration["MonitorPort"], out var monitorPort))
                {
                    monitorPort = 5002;
                }

                var result = httpClient.GetAsync($"{serverDomain}/api/agents/heartbeat?agentAddress={clientAddress}&agentPort={agentPort}&monitorPort={monitorPort}").Result;

                if (!result.IsSuccessStatusCode)
                {
                    var content = result.Content.ReadAsStringAsync().Result;
                    _logger.LogError(content);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Send heartbeat test hosted service] is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
