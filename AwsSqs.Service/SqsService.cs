using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AwsSqs.Service
{
    public class SqsService : ISqsService
    {
        private readonly IAmazonSQS _amazonSqs;

        public SqsService(IAmazonSQS amazonSqs)
        {
            _amazonSqs = amazonSqs;
        }

        public async Task<SendMessageResponse> EnviarMensagemSqsAsync(string queueUrl, string messageBody,
            string messageDeduplicationId, string messageGroupId)
        {
            var sendRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = messageBody
            };

            if (!FilaFifo(queueUrl)) return await _amazonSqs.SendMessageAsync(sendRequest);

            sendRequest.MessageDeduplicationId = messageDeduplicationId;
            sendRequest.MessageGroupId = messageGroupId;

            return await _amazonSqs.SendMessageAsync(sendRequest);
        }

        public async Task<IEnumerable<Message>> ConsumirMensagemSqsAsync(string queueUrl, int maxNumberOfMessages,
            int waitTimeSeconds, CancellationToken stoppingToken)
        {
            try
            {
                var receiveMessageRequest = new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = maxNumberOfMessages,
                    WaitTimeSeconds = waitTimeSeconds
                };
                var response = await _amazonSqs.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);
                return response.Messages;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public async Task<DeleteMessageResponse> DeletarMensagemSqsAsync(string queueUrl, Message message,
            CancellationToken stoppingToken)
        {
            var deleteMessageRequest = new DeleteMessageRequest
            {
                QueueUrl = queueUrl,
                ReceiptHandle = message.ReceiptHandle
            };
            var result = await _amazonSqs.DeleteMessageAsync(deleteMessageRequest, stoppingToken);
            return result;
        }

        private static bool FilaFifo(string queueUrl)
        {
            if (string.IsNullOrEmpty(queueUrl))
                return false;

            return queueUrl.Length > 5 && queueUrl.Substring(queueUrl.Length - 5).Equals(".fifo");
        }
    }
}
