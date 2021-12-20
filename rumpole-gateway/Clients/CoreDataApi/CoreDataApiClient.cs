using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Domain.CoreDataApi.CaseDetails;
using RumpoleGateway.Domain.CoreDataApi.ResponseTypes;
using RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory;
using System;
using System.Threading.Tasks;

namespace RumpoleGateway.Clients.CoreDataApi
{
    public class CoreDataApiClient : ICoreDataApiClient
    {
        private readonly IGraphQLClient _coreDataApiClient;
        private readonly IAuthenticatedGraphQLHttpRequestFactory _authenticatedGraphQLHttpRequestFactory;
        private readonly ILogger<CoreDataApiClient> _logger;
        public CoreDataApiClient(IGraphQLClient coreDataApiClient,
            IAuthenticatedGraphQLHttpRequestFactory authenticatedGraphQLHttpRequestFactory,
            ILogger<CoreDataApiClient> logger)
        {
            _coreDataApiClient = coreDataApiClient;
            _authenticatedGraphQLHttpRequestFactory = authenticatedGraphQLHttpRequestFactory;
            _logger = logger;
        }
         
        public async Task<CaseDetails> GetCaseDetailsById(string caseId, string accessToken)
        {
            try
            {
                var query = new GraphQLHttpRequest
                {
                    Query = "query {case(id: " + caseId + ")  {id uniqueReferenceNumber caseType  appealType leadDefendant {firstNames surname organisationName}  offences { earlyDate lateDate listOrder code shortDescription longDescription }  }}"
                };

                //var query = new GraphQLHttpRequest
                //{
                //    Query = @"
                //        query casesQuery($id: caseUrn!) {
                //          case(id: $id) {
                //            id
                //            uniqueReferenceNumber
                //            caseType
                //            appealType
                //            //accounts {
                //            //  id
                //            //  type
                //            //  description
                //            //}
                //          }
                //        }",
                //    Variables = new { id = caseId }
                //};

                var authenticatedRequest = _authenticatedGraphQLHttpRequestFactory.Create(accessToken, query);
                _logger.LogInformation($" Token  -   {accessToken} ");
                var response = await _coreDataApiClient.SendQueryAsync<ResponseCaseDetails>(authenticatedRequest);
                _logger.LogInformation($" response  -   {response.Data.CaseDetails.UniqueReferenceNumber} ");
                return response.Data.CaseDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError($" Error -  response from Data Core API -  {ex} ");
                return null;
            }
        }
    }
}
