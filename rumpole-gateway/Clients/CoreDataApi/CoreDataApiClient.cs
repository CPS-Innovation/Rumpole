using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using RumpoleGateway.Domain.CoreDataApi;
using RumpoleGateway.Domain.CoreDataApi.CaseDetails;
using RumpoleGateway.Domain.CoreDataApi.ResponseTypes;
using RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Clients.CoreDataApi
{
    public class CoreDataApiClient : ICoreDataApiClient
    {
        private readonly IGraphQLClient _coreDataApiClient;
        private readonly IAuthenticatedGraphQlHttpRequestFactory _authenticatedGraphQlHttpRequestFactory;
        private readonly ILogger<CoreDataApiClient> _logger;
        public CoreDataApiClient(IGraphQLClient coreDataApiClient, IAuthenticatedGraphQlHttpRequestFactory authenticatedGraphQlHttpRequestFactory, 
            ILogger<CoreDataApiClient> logger)
        {
            _coreDataApiClient = coreDataApiClient;
            _authenticatedGraphQlHttpRequestFactory = authenticatedGraphQlHttpRequestFactory;
            _logger = logger;
        }
         
        public async Task<CaseDetails> GetCaseDetailsByIdAsync(string caseId, string accessToken, Guid correlationId)
        {
            _logger.LogMethodEntry(correlationId, nameof(GetCaseDetailsByIdAsync), $"For CaseId: {caseId}");
            CaseDetails caseDetails = null;
            
            try
            {
                var query = new GraphQLHttpRequest
                {
                    Query = "query {case(id: " + caseId + ")  {id uniqueReferenceNumber caseType  appealType caseStatus {code description } "
                            + " leadDefendant {firstNames surname organisationName}  " 
                            + " offences { earlyDate lateDate listOrder code shortDescription longDescription }  }}"
                };

                var authenticatedRequest = _authenticatedGraphQlHttpRequestFactory.Create(accessToken, query, correlationId);
                
                _logger.LogMethodFlow(correlationId, nameof(GetCaseDetailsByIdAsync), $"Sending the following query to the Core Data API: {query.ToJson()}");
                var response = await _coreDataApiClient.SendQueryAsync<ResponseCaseDetails>(authenticatedRequest);
                
                if (response.Data?.CaseDetails == null) return null;

                caseDetails = response.Data.CaseDetails;
                return caseDetails;
            }
            catch (Exception exception)
            {
                throw new CoreDataApiException("Error response from Core Data Api.", exception);
            }
            finally
            {
                _logger.LogMethodExit(correlationId, nameof(GetCaseDetailsByIdAsync), caseDetails.ToJson());
            }
        }

        public async Task<IList<CaseDetails>> GetCaseInformationByUrnAsync(string urn, string accessToken, Guid correlationId)
        {
            _logger.LogMethodEntry(correlationId, nameof(GetCaseInformationByUrnAsync), $"For Urn: {urn}");
            IList<CaseDetails> caseDetailsCollection = null;
            try
            {
                var query = new GraphQLHttpRequest
                {
                    Query = "query {cases(urn: \"" + urn + "\")  "
                            + " {id uniqueReferenceNumber caseType  appealType caseStatus {code description } "
                            + " leadDefendant {firstNames surname organisationName}"
                            + " offences { earlyDate lateDate listOrder code shortDescription longDescription }  }}"
                };

                var authenticatedRequest = _authenticatedGraphQlHttpRequestFactory.Create(accessToken, query, correlationId);
                authenticatedRequest.Add("Correlation-Id", correlationId.ToString());

                _logger.LogMethodFlow(correlationId, nameof(GetCaseDetailsByIdAsync), $"Sending the following query to the Core Data API: {query.ToJson()}");
                var response = await _coreDataApiClient.SendQueryAsync<ResponseCaseInformationByUrn>(authenticatedRequest);

                if (response.Data == null || response.Data?.CaseDetails?.Count == 0) return null;

                if (response.Data != null) caseDetailsCollection = response.Data.CaseDetails;
                return caseDetailsCollection;
            }
            catch (Exception exception)
            {
                throw new CoreDataApiException("Error response from the Core Data Api.", exception);
            }
            finally
            {
                _logger.LogMethodExit(correlationId, nameof(GetCaseInformationByUrnAsync), caseDetailsCollection.ToJson());
            }
        }
    }
}
