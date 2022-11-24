using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RumpoleGateway.Domain.CaseData.Args;
using RumpoleGateway.Exceptions;
using RumpoleGateway.CaseDataImplementations.Tde.Domain;
using RumpoleGateway.CaseDataImplementations.Tde.Factories;
using RumpoleGateway.Wrappers;

namespace RumpoleGateway.CaseDataImplementations.Tde.Clients
{
    public class TdeClient : ITdeClient
    {
        private readonly HttpClient _httpClient;
        private readonly ITdeClientRequestFactory _tdeClientRequestFactory;
        private readonly IJsonConvertWrapper _jsonConvertWrapper;

        public TdeClient(HttpClient httpClient, ITdeClientRequestFactory tdeClientRequestFactory, IJsonConvertWrapper jsonConvertWrapper)
        {
            _httpClient = httpClient;
            _tdeClientRequestFactory = tdeClientRequestFactory;
            _jsonConvertWrapper = jsonConvertWrapper;
        }

        public async Task<IEnumerable<CaseIdentifiers>> ListCaseIdsAsync(UrnArg arg) =>
            await CallTde<IEnumerable<CaseIdentifiers>>(
                () => _tdeClientRequestFactory.CreateListCasesRequest(arg),
                 arg.CorrelationId
            );


        public async Task<Case> GetCaseAsync(CaseArg arg) =>
            await CallTde<Case>(
                () => _tdeClientRequestFactory.CreateGetCaseRequest(arg),
                arg.CorrelationId
            );


        public async Task<IEnumerable<DocumentDetails>> ListCaseDocumentsAsync(CaseArg arg) =>
            await CallTde<IEnumerable<DocumentDetails>>(
                () => _tdeClientRequestFactory.CreateListCaseDocumentsRequest(arg),
                 arg.CorrelationId
            );

        private async Task<T> CallTde<T>(Func<HttpRequestMessage> requestFactory, Guid correlationId)
        {
            using var response = await _httpClient.SendAsync(requestFactory());
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException exception)
            {
                throw new HttpException(response.StatusCode, exception);
            }

            var content = await response.Content.ReadAsStringAsync();

            return _jsonConvertWrapper.DeserializeObject<T>(content, correlationId);
        }
    }
}