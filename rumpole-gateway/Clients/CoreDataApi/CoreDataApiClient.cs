using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Domain.CoreDataApi;
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
        //public async Task<CaseInformation> GetCaseInformationByUrn(string urn)
        //{

        //    try
        //    {
        //        var query = new GraphQLRequest
        //        {
        //            Query = @"
        //                query casesQuery($urn: caseUrn!) {
        //                  cases(urn: $urn) {
        //                    id
        //                    uniqueReferenceNumber
        //                    caseType
        //                    appealType
        //                    //accounts {
        //                    //  id
        //                    //  type
        //                    //  description
        //                    //}
        //                  }
        //                }",
        //            Variables = new { urn = urn }
        //        };

        //        var response = await _coreDataApiClient.SendMutationAsync<ResponseCaseInformation>(query);
        //        return response.Data.CaseInformation;
        //    }
        //    catch (Exception ex)
        //    {
        //        string error = ex.ToString();
        //        return null;
        //    }
        //}
        public async Task<CaseInformation> GetCaseInformationByUrn(string urn, string accessToken)
        {




            try
            {
                var query = new GraphQLHttpRequest
                {
                    //Query = @"
                    //    query  cases(urn: $urn) {
                    //        id
                    //        uniqueReferenceNumber
                    //        caseType
                    //        appealType
                    //      }
                    //    }",
                    Query = @"
                query cases($urn: String!) {
                  cases(urn: '13WD1234520') {
                    id
                    uniqueReferenceNumber
                    caseType
                    appealType
                  }
                }",
                    Variables = new { urn = urn }
                };

                var authenticatedRequest = _authenticatedGraphQLHttpRequestFactory.Create(accessToken, query);
                _logger.LogInformation($" Success - authenticatedRequest : {accessToken}");
                var response = await _coreDataApiClient.SendQueryAsync<ResponseCaseInformation>(authenticatedRequest);
                _logger.LogInformation($" Success response from Data Core API - {response.Data} ");
                return response.Data.CaseInformation;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($" Error -  response from Data Core API -  {ex.ToString()} ");
                string error = ex.ToString();
                return null;
            }
        }

    }
}
