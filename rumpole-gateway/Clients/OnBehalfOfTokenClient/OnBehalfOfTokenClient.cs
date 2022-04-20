
using Microsoft.Identity.Client;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RumpoleGateway.Clients.OnBehalfOfTokenClient
{
    class OnBehalfOfTokenClient : IOnBehalfOfTokenClient
    {
        private readonly IConfidentialClientApplication _application;
     
        public OnBehalfOfTokenClient(IConfidentialClientApplication application)
        {
            _application = application;
        }

        public async Task<string> GetAccessTokenAsync(string accessToken, string scope)
        {
            var scopes = new Collection<string> { scope };
            var userAssertion = new UserAssertion(accessToken, Constants.Authentication.AzureAuthenticationAssertionType);
            var result = await _application.AcquireTokenOnBehalfOf(scopes, userAssertion).ExecuteAsync();
            return result.AccessToken;
        }
    }
}
