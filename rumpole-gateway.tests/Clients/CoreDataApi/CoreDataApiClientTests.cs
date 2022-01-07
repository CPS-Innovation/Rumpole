//using System.Threading.Tasks;
//using GraphQL;
//using GraphQL.Client.Abstractions;
//using GraphQL.Client.Http;
//using Microsoft.Extensions.Logging;
//using Moq;
//using NSubstitute;
//using RumpoleGateway.Clients.CoreDataApi;
//using RumpoleGateway.Domain.CoreDataApi.ResponseTypes;
//using RumpoleGateway.Extensions;
//using RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory;
//using RumpoleGateway.Tests.FakeData;
//using Xunit;

//namespace RumpoleGateway.Tests.Clients.CoreDataApi
//{
//    public class CoreDataApiClientTests : IClassFixture<ResponseCaseDetailsFake>
//    {
//        private readonly Mock<IGraphQLClient> _coreDataApiClientMock;
//        private readonly Mock<IAuthenticatedGraphQLHttpRequestFactory> _authenticatedGraphQLHttpRequestFactoryMock;
//        private readonly Mock<ILogger<CoreDataApiClient>> _loggerMock;
//        private readonly ResponseCaseDetailsFake _responseCaseDetailsFake;

//        public CoreDataApiClientTests()
//        {
//            _coreDataApiClientMock = new Mock<IGraphQLClient>();
//            _authenticatedGraphQLHttpRequestFactoryMock = new Mock<IAuthenticatedGraphQLHttpRequestFactory>();
//            _loggerMock = new Mock<ILogger<CoreDataApiClient>>();
//        }
//        public CoreDataApiClientTests(ResponseCaseDetailsFake responseCaseDetailsFake)
//        {
//            _responseCaseDetailsFake = responseCaseDetailsFake;
//        }

//        [Fact]
//        public async Task CoreDataApiClient_GetCaseDetailsById_Should_Return_Response_Valid_response()
//        {
//            //Arrange
//            _authenticatedGraphQLHttpRequestFactoryMock.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<GraphQLHttpRequest>()))
//                                                                    .Returns(new AuthenticatedGraphQLHttpRequest(It.IsAny<string>(), It.IsAny<GraphQLHttpRequest>()));

            
//            _coreDataApiClientMock.Setup(x => x.SendQueryAsync(request,default)
//                                        .ReturnsAsync(_responseCaseDetailsFake.GetCaseDetailsResponse_Payload()));
//            //_coreDataApiClientMock.SendQueryAsync<ResponseCaseDetails>(null)
//            //    .Returns(Task.FromResult(GraphQL.GraphQLResponse<_responseCaseDetailsFake.GetCaseDetailsResponse_Payload()>));
            
//            var coreDataApiClient = GetCoreDataApiClient();

//            //Act
//            var results = await coreDataApiClient.GetCaseDetailsById(It.IsAny<string>(), It.IsAny<string>());

//            //Assert
//            //Assert.Equal(401, results.StatusCode);
//            //Assert.Equal(Constants.CommonUserMessages.AuthenticationFailedMessage, results.Value);
//        }

//        #region private methods
//        //private CoreDataApiClient GetCoreDataApiClient()
//        //{
//        //    return new CoreDataApiClient(_coreDataApiClientMock,
//        //                                 _authenticatedGraphQLHttpRequestFactoryMock,
//        //                                 _loggerMock)
//        //    { };
//        //}
//        #endregion private methods
//    }

//}
