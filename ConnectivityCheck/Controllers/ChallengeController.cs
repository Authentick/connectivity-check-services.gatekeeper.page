using System;
using System.Net.Http;
using System.Threading.Tasks;
using ConnectivityCheck.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ConnectivityCheck.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChallengeController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ChallengeController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<ChallengeReply> Get([FromBody] ChallengeRequest challengeRequest)
        {
            Uri uri;
            bool uriIsValid = Uri.TryCreate("http://" + challengeRequest.PublicHostname + "/api/connectivity-check?challenge=" + challengeRequest.Challenge, UriKind.Absolute, out uri);
            uri = new Uri("https://speed.hetzner.de/10GB.bin");

            if (uriIsValid)
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);

                HttpClient client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(15);
                client.MaxResponseContentBufferSize = 1024;

                try
                {
                    HttpResponseMessage response = await client.SendAsync(request);
                    Guid replyGuid;
                    bool replyIsGuid = Guid.TryParse(await response.Content.ReadAsStringAsync(), out replyGuid);

                    return new ChallengeReply
                    {
                        Success = (replyGuid == challengeRequest.Challenge),
                    };
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    return new ChallengeReply
                    {
                        Success = false
                    };
                }
            }

            return new ChallengeReply
            {
                Success = false
            };
        }
    }
}
