using GraphQL;
using GraphQL.Client.Abstractions;
using RumpoleGateway.Domain.CoreDataApi;
using RumpoleGateway.Domain.CoreDataApi.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RumpoleGateway.Clients.CoreDataApi
{
    public class CoreDataApiClient : ICoreDataApiClient
    {
        private readonly IGraphQLClient _coreDataApiClient;

        public CoreDataApiClient(IGraphQLClient coreDataApiClient)
        {
            _coreDataApiClient = coreDataApiClient;
        }
        public async Task<CaseInformation> GetCaseInformationByUrn()
        {
            //var query = new GraphQLRequest
            //{
            //    Query = @"
            //    mutation($owner: ownerInput!){
            //      createOwner(owner: $owner){
            //        id,
            //        name,
            //        address
            //      }
            //    }",
            //    Variables = new { owner = ownerToCreate }
            //};
            //var response = await _coreDataApiClient.SendMutationAsync<ResponseCaseInformation>(query);
            //return response.Data.Owner;

            //var query = new GraphQLRequest
            //{
            //    Query = @"
            //            query ownersQuery{
            //              owners {
            //                id
            //                name
            //                address
            //                accounts {
            //                  id
            //                  type
            //                  description
            //                }
            //              }
            //            }",
            //};

            //var response = await _coreDataApiClient.SendQueryAsync<ResponseCaseInformation>(query);
            //return response.Data.CaseInformation;
            return null;
        }
    }
}
