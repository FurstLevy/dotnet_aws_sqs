using Amazon.SQS.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AwsSqs.Service
{
    public interface ISqsService
    {
        Task<SendMessageResponse> EnviarMensagemSqsAsync(string queueUrl, string messageBody,
            string messageDeduplicationId, string messageGroupId);

        Task<IEnumerable<Message>> ConsumirMensagemSqsAsync(string queueUrl, int maxNumberOfMessages, int waitTimeSeconds,
            CancellationToken stoppingToken);

        Task<DeleteMessageResponse> DeletarMensagemSqsAsync(string queueUrl, Message message,
            CancellationToken stoppingToken);
    }
}
