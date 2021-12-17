using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace RumpoleGateway.Clients.OnBehalfOfTokenClient
{
    class OnBehalfOfTokenClient : IOnBehalfOfTokenClient
    {
        private IConfidentialClientApplication _application;
        private readonly ILogger<OnBehalfOfTokenClient> _log;

        private const string assertionType = "urn:ietf:params:oauth:grant-type:jwt-bearer";
        //private readonly ICollection<string> scopes = new Collection<string> { "group.read.all" };
        // private readonly ICollection<string> scopes = new Collection<string> { "api://637b9bcb-395d-4bfc-bedc-3302e5744e84/caseconfirmation.readwrite.all" };
        private readonly ICollection<string> scopes = new Collection<string> { "api://5f1f433a-41b3-45d3-895e-927f50232a47/case.confirm" }; //cps-core-data-api-dev  -- scope

        public OnBehalfOfTokenClient(IConfidentialClientApplication application, ILogger<OnBehalfOfTokenClient> log)
        {
            _application = application;
            _log = log;
        }

        public async Task<string> GetAccessToken(string accessToken)
        {
            AuthenticationResult result;

            try
            {
                var userAssertion = new UserAssertion(accessToken, assertionType);
                result = await _application.AcquireTokenOnBehalfOf(scopes, userAssertion).ExecuteAsync();
            }
            catch (MsalException exception)
            {
                var baseMessage = "Failed to acquire onBehalfOf token";
                _log.LogError(exception, baseMessage);

                //TODO:
                throw new Exception();
               // throw new OnBehalfOfTokenClientException(baseMessage);
            }


            return result.AccessToken;
        }
    }
}
