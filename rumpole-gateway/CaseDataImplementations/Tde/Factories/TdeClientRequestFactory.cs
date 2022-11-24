using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using RumpoleGateway.Domain.CaseData.Args;
using RumpoleGateway.CaseDataImplementations.Tde.Options;

namespace RumpoleGateway.CaseDataImplementations.Tde.Factories
{
    public class TdeClientRequestFactory : ITdeClientRequestFactory
    {
        private const string _correlationId = "Correlation-Id";
        private const string _functionKey = "x-functions-key";

        private readonly TdeOptions _options;

        public TdeClientRequestFactory(IOptions<TdeOptions> tdeOptions)
        {
            _options = tdeOptions.Value;
        }

        public HttpRequestMessage CreateListCasesRequest(UrnArg arg)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/urns/{arg.Urn}/cases");
            AddAuthHeaders(request, arg);
            return request;
        }

        public HttpRequestMessage CreateGetCaseRequest(CaseArg arg)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/urns/{arg.Urn}/cases/{arg.CaseId}");
            AddAuthHeaders(request, arg);
            return request;
        }

        public HttpRequestMessage CreateListCaseDocumentsRequest(CaseArg arg)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/urns/{arg.Urn}/cases/{arg.CaseId}/documents");
            AddAuthHeaders(request, arg);
            return request;
        }

        private void AddAuthHeaders(HttpRequestMessage request, BaseCaseDataArg arg)
        {
            request.Headers.Add(AuthenticationKeys.Authorization, $"{AuthenticationKeys.Bearer} {arg.OnBehalfOfToken}");
            request.Headers.Add(AuthenticationKeys.UpstreamTokenName, arg.UpstreamToken);
            if (!string.IsNullOrEmpty(_options.AccessKey))
            {
                request.Headers.Add(_functionKey, _options.AccessKey);
            }
            request.Headers.Add(_correlationId, arg.CorrelationId.ToString());
        }
    }
}