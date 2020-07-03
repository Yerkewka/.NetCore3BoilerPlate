using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces.Services.Notification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Sms
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<SmsService> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<bool> SendAsync(string mobileNumber, string content, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Sending sms to {mobileNumber} with text {content}");

            var client = _httpClientFactory.CreateClient("SmsService");

            var login = _configuration["Sms:Login"];
            var password = _configuration["Sms:Password"];

            var response = await client
                .GetAsync($"/sys/send.php?login={login}&psw={password}&phones={mobileNumber}&mes={content}&charset=utf-8&fmt=3", cancellationToken);

            return response.IsSuccessStatusCode;
        }
    }
}
