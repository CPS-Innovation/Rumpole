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
             var scopes = new Collection<string> { _configuration["CoreDataApiScope"] };
            try
            {
                var userAssertion = new UserAssertion(accessToken, Constants.Authentication.AzureAuthenticationAssertionType);
                result = await _application.AcquireTokenOnBehalfOf(scopes, userAssertion).ExecuteAsync();
            }
            catch (MsalException exception)
            {
                _logger.LogError(exception, "Failed to acquire onBehalfOf token");

                throw;
            }

            return result.AccessToken;
        }
    }
}
