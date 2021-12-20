using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RumpoleGateway.Clients.OnBehalfOfTokenClient
{
    class OnBehalfOfTokenClient : IOnBehalfOfTokenClient
    {
        private readonly IConfidentialClientApplication _application;
        private readonly ILogger<OnBehalfOfTokenClient> _logger;
        private readonly IConfiguration _configuration;
     
        public OnBehalfOfTokenClient(IConfidentialClientApplication application, 
                                     ILogger<OnBehalfOfTokenClient> logger,
                                     IConfiguration configuration)
        {
            _application = application;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> GetAccessToken(string accessToken)
        {
            AuthenticationResult result;
            // var scopes = new Collection<string> { _configuration["CoreDataApiScope"] };
            var scopes = new Collection<string> { "api://5f1f433a-41b3-45d3-895e-927f50232a47/case.confirm" };
            try
            {
                var userAssertion = new UserAssertion(accessToken, Constants.Authentication.AzureAuthenticationAssertionType);
                result = await _application.AcquireTokenOnBehalfOf(scopes, userAssertion).ExecuteAsync();
            }
            catch (MsalException exception)
            {
                var baseMessage = "Failed to acquire onBehalfOf token";
                _logger.LogError(exception, baseMessage);

                //TODO:
                throw new Exception();
               // throw new OnBehalfOfTokenClientException(baseMessage);
            }

            return result.AccessToken;
        }
    }
}
