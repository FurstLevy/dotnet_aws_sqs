using Amazon.SQS.Model;
using AwsSqs.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AwsSqs.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISqsService _sqsService;
        private readonly IConfigurationRoot _configurationRoot;

        public Worker(ILogger<Worker> logger, ISqsService sqsService, IConfigurationRoot configurationRoot)
        {
            _logger = logger;
            _sqsService = sqsService;
            _configurationRoot = configurationRoot;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var mensagens = await _sqsService.ConsumirMensagemSqsAsync(_configurationRoot["AWS:queueUrl"],
                    Convert.ToInt32(_configurationRoot["AWS:MaxNumberOfMessages"]),
                    Convert.ToInt32(_configurationRoot["AWS:WaitTimeSeconds"]),
                    stoppingToken);

                foreach (var mensagem in mensagens)
                {
                    ProcessarMensagem(mensagem, stoppingToken);
                }
            }
        }

        private void ProcessarMensagem(Message mensagem, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Msg: {mensagem.Body} | {DateTime.UtcNow}");
            _sqsService.DeletarMensagemSqsAsync(_configurationRoot["AWS:queueUrl"], mensagem, stoppingToken);
        }
    }
}
