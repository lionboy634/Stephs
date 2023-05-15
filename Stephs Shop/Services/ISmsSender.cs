using Microsoft.Extensions.Options;
using Stephs_Shop.Models.Options;
using System.Net.Http;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Stephs_Shop.Services
{
    public interface ISmsSender
    {
        public Task SendMessage(string message, string recipient, string subject);
    }

    public class SmsSender : ISmsSender
    {
        private readonly HttpClient _client;
        private InfoBipOptions _options;
        private readonly ILogger<SmsSender> _logger;
        public SmsSender(IOptions<InfoBipOptions> option)
        {
            _client = new HttpClient();
            _options = option.Value;
        }
        public async Task SendMessage(string message , string recipient, string subject)
        {
            _client.BaseAddress = new Uri(_options.BaseUrl);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_options.ApiKey);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            string message_to = $@"
            {{
                ""messages"": [
                {{
                    ""from"": ""{_options.EmailSender}"",
                    ""destinations"":
                    [
                        {{
                            ""to"": ""{recipient}""
                        }}
                  ],
                  ""text"": ""{subject}""
                }}
              ]
            }}";

            var responseContent = "";
            try
            {
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "sms/2/text/advanced");
                httpRequest.Content = new StringContent(message, Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(httpRequest);
                responseContent = await response.Content.ReadAsStringAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError($"Error Sending Email --- {responseContent}");
            }
           


        }
    }
}
