using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using RumpoleGateway.Domain.CoreDataApi;
using RumpoleGateway.Domain.CoreDataApi.CaseDetails;
using RumpoleGateway.Domain.CoreDataApi.ResponseTypes;
using RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RumpoleGateway.Clients.CoreDataApi
{
    public class CoreDataApiClient : ICoreDataApiClient
    {
        private readonly IGraphQLClient _coreDataApiClient;
        private readonly IAuthenticatedGraphQLHttpRequestFactory _authenticatedGraphQLHttpRequestFactory;
        public CoreDataApiClient(IGraphQLClient coreDataApiClient,
            IAuthenticatedGraphQLHttpRequestFactory authenticatedGraphQLHttpRequestFactory)
        {
            _coreDataApiClient = coreDataApiClient;
            _authenticatedGraphQLHttpRequestFactory = authenticatedGraphQLHttpRequestFactory;
        }
         
        public async Task<CaseDetails> GetCaseDetailsByIdAsync(string caseId, string accessToken)
        {
            try
            {
                var query = new GraphQLHttpRequest
                {
                    Query = "query {case(id: " + caseId + ")  {id uniqueReferenceNumber caseType  appealType caseStatus {code description } "
                            + " leadDefendant {firstNames surname organisationName}  " 
                            + " offences { earlyDate lateDate listOrder code shortDescription longDescription }  }}"
                };

                var authenticatedRequest = _authenticatedGraphQLHttpRequestFactory.Create(accessToken, query);
                var response = await _coreDataApiClient.SendQueryAsync<ResponseCaseDetails>(authenticatedRequest);
                
                if (response.Data == null || response.Data?.CaseDetails == null) return null;

                return response.Data.CaseDetails;
            }
            catch (Exception exception)
            {
                //TODO test
                throw new CoreDataApiException("Error response from Core Data Api.", exception);
            }
        }

        public async Task<IList<CaseDetails>> GetCaseInformationByUrnAsync(string urn, string accessToken)
        {
            try
            {

                var query = new GraphQLHttpRequest
                {
                    Query = "query {cases(urn: \""+ urn +"\")  " 
                             + " {id uniqueReferenceNumber caseType  appealType caseStatus {code description } "
                             + " leadDefendant {firstNames surname organisationName}"
                             + " offences { earlyDate lateDate listOrder code shortDescription longDescription }  }}"
                };

                var authenticatedRequest = _authenticatedGraphQLHttpRequestFactory.Create(accessToken, query);
                var response = await _coreDataApiClient.SendQueryAsync<ResponseCaseInformationByUrn>(authenticatedRequest);
                
                if (response.Data == null || response.Data?.CaseDetails?.Count == 0) return null;
                
                return response.Data.CaseDetails;
            }
            catch (Exception exception)
            {
                //TODO test
                throw new CoreDataApiException("Error response from Core Data Api.", exception);
            }
        }
    }
}
