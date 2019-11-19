using AwsSqs.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwsSqs.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SqsController : ControllerBase
    {
        private readonly ISqsService _sqsService;
        private readonly string _queueUrl;

        public SqsController(ISqsService sqsService, IConfiguration configuration)
        {
            _sqsService = sqsService;
            _queueUrl = configuration["AWS:queueUrl"];
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> Inserir([FromBody] SqsViewModel requestBody)
        {
            await _sqsService.EnviarMensagemSqsAsync(
                queueUrl: _queueUrl,
                messageBody: JsonSerializer.Serialize(requestBody),
                messageDeduplicationId: Guid.NewGuid().ToString(),
                messageGroupId: "AwsSqsApi");

            return Ok();
        }
    }

    public class SqsViewModel
    {
        public string Nome { get; set; }
        public int Idade { get; set; }
    }
}
