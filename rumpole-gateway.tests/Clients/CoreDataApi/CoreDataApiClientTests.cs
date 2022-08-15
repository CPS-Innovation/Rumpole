using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using Moq;
using RumpoleGateway.Clients.CoreDataApi;
using RumpoleGateway.Domain.CoreDataApi.CaseDetails;
using RumpoleGateway.Domain.CoreDataApi.ResponseTypes;
using RumpoleGateway.Extensions;
using RumpoleGateway.Factories.AuthenticatedGraphQLHttpRequestFactory;
using RumpoleGateway.Tests.FakeData;
using Xunit;

namespace RumpoleGateway.Tests.Clients.CoreDataApi
{
    public class CoreDataApiClientTests : IClassFixture<ResponseCaseDetailsFake>
    {
        private readonly Mock<IGraphQLClient> _coreDataApiClientMock;
        private readonly Mock<IAuthenticatedGraphQLHttpRequestFactory> _authenticatedGraphQlHttpRequestFactoryMock;
        private readonly ResponseCaseDetailsFake _responseCaseDetailsFake;
        private readonly Fixture _fixture;

        public CoreDataApiClientTests()
        {
            _coreDataApiClientMock = new Mock<IGraphQLClient>();
            _authenticatedGraphQlHttpRequestFactoryMock = new Mock<IAuthenticatedGraphQLHttpRequestFactory>();
            _fixture = new Fixture();
            _responseCaseDetailsFake = new ResponseCaseDetailsFake();
        }
        
        [Fact]
        public async Task CoreDataApiClient_GetCaseDetailsById_Should_Return_Response_Valid_response()
        {
            //Arrange
            _authenticatedGraphQlHttpRequestFactoryMock.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<GraphQLHttpRequest>()))
                                                                    .Returns(new AuthenticatedGraphQlHttpRequest(_fixture.Create<string>(), _fixture.Create<GraphQLHttpRequest>()));

            var fakedResponse = new GraphQLResponse<ResponseCaseDetails>
            {
                Data = _responseCaseDetailsFake.GetCaseDetailsResponse_Payload()
            };
            _coreDataApiClientMock.Setup(x => x.SendQueryAsync<ResponseCaseDetails>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(fakedResponse);
            
            var coreDataApiClient = GetCoreDataApiClient();

            //Act
            var results = await coreDataApiClient.GetCaseDetailsByIdAsync(_fixture.Create<string>(), _fixture.Create<string>());

            //Assert
            results.Id.Should().Be(fakedResponse.Data.CaseDetails.Id);
        }

        [Fact]
        public async Task CoreDataApiClient_GetCaseDetailsById_WhenResponseData_IsNull_ReturnsNull()
        {
            //Arrange
            _authenticatedGraphQlHttpRequestFactoryMock.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<GraphQLHttpRequest>()))
                .Returns(new AuthenticatedGraphQlHttpRequest(_fixture.Create<string>(), _fixture.Create<GraphQLHttpRequest>()));

            var fakedResponse = new GraphQLResponse<ResponseCaseDetails>
            {
                Data = null
            };
            _coreDataApiClientMock.Setup(x => x.SendQueryAsync<ResponseCaseDetails>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(fakedResponse);

            var coreDataApiClient = GetCoreDataApiClient();

            //Act
            var results = await coreDataApiClient.GetCaseDetailsByIdAsync(_fixture.Create<string>(), _fixture.Create<string>());

            //Assert
            results.Should().BeNull();
        }

        [Fact]
        public async Task CoreDataApiClient_GetCaseDetailsById_WhenCaseDetails_IsNull_ReturnsNull()
        {
            //Arrange
            _authenticatedGraphQlHttpRequestFactoryMock.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<GraphQLHttpRequest>()))
                .Returns(new AuthenticatedGraphQlHttpRequest(_fixture.Create<string>(), _fixture.Create<GraphQLHttpRequest>()));

            var fakedResponse = new GraphQLResponse<ResponseCaseDetails>
            {
                Data = _responseCaseDetailsFake.GetCaseDetailsResponse_Payload()
            };
            fakedResponse.Data.CaseDetails = null;
            _coreDataApiClientMock.Setup(x => x.SendQueryAsync<ResponseCaseDetails>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(fakedResponse);

            var coreDataApiClient = GetCoreDataApiClient();

            //Act
            var results = await coreDataApiClient.GetCaseDetailsByIdAsync(_fixture.Create<string>(), _fixture.Create<string>());

            //Assert
            results.Should().BeNull();
        }

        [Fact]
        public async Task CoreDataApiClient_GetCaseDetailsById_WhenCaseDetails_ThrowsException_IsCaughtSuccessfully()
        {
            //Arrange
            _authenticatedGraphQlHttpRequestFactoryMock.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<GraphQLHttpRequest>()))
                .Returns(new AuthenticatedGraphQlHttpRequest(_fixture.Create<string>(), _fixture.Create<GraphQLHttpRequest>()));

            var fakedResponse = new GraphQLResponse<ResponseCaseDetails>
            {
                Data = _responseCaseDetailsFake.GetCaseDetailsResponse_Payload()
            };
            fakedResponse.Data.CaseDetails = null;
            _coreDataApiClientMock.Setup(x => x.SendQueryAsync<ResponseCaseDetails>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            var coreDataApiClient = GetCoreDataApiClient();

            //Act
            var results = async () => await coreDataApiClient.GetCaseDetailsByIdAsync(_fixture.Create<string>(), _fixture.Create<string>());

            //Assert
            await results.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CoreDataApiClient_GetCaseInformationByUrnAsync_Should_Return_Response_Valid_response()
        {
            //Arrange
            _authenticatedGraphQlHttpRequestFactoryMock.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<GraphQLHttpRequest>()))
                                                                    .Returns(new AuthenticatedGraphQlHttpRequest(_fixture.Create<string>(), _fixture.Create<GraphQLHttpRequest>()));

            var fakedResponse = _fixture.Create<GraphQLResponse<ResponseCaseInformationByUrn>>();
            fakedResponse.Data.CaseDetails = _fixture.CreateMany<CaseDetails>(5).ToList();
            _coreDataApiClientMock.Setup(x => x.SendQueryAsync<ResponseCaseInformationByUrn>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(fakedResponse);

            var coreDataApiClient = GetCoreDataApiClient();

            //Act
            var results = await coreDataApiClient.GetCaseInformationByUrnAsync(_fixture.Create<string>(), _fixture.Create<string>());

            //Assert
            results.Count.Should().Be(5);
        }

        [Fact]
        public async Task CoreDataApiClient_GetCaseInformationByUrnAsync_WhenResponseData_IsNull_ReturnsNull()
        {
            //Arrange
            _authenticatedGraphQlHttpRequestFactoryMock.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<GraphQLHttpRequest>()))
                .Returns(new AuthenticatedGraphQlHttpRequest(_fixture.Create<string>(), _fixture.Create<GraphQLHttpRequest>()));

            var fakedResponse = _fixture.Create<GraphQLResponse<ResponseCaseInformationByUrn>>();
            fakedResponse.Data = null;
            _coreDataApiClientMock.Setup(x => x.SendQueryAsync<ResponseCaseInformationByUrn>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(fakedResponse);

            var coreDataApiClient = GetCoreDataApiClient();

            //Act
            var results = await coreDataApiClient.GetCaseInformationByUrnAsync(_fixture.Create<string>(), _fixture.Create<string>());

            //Assert
            results.Should().BeNull();
        }

        [Fact]
        public async Task CoreDataApiClient_GetCaseInformationByUrnAsync_WhenCaseDetails_IsNull_ReturnsNull()
        {
            _authenticatedGraphQlHttpRequestFactoryMock.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<GraphQLHttpRequest>()))
                .Returns(new AuthenticatedGraphQlHttpRequest(_fixture.Create<string>(), _fixture.Create<GraphQLHttpRequest>()));

            var fakedResponse = _fixture.Create<GraphQLResponse<ResponseCaseInformationByUrn>>();
            fakedResponse.Data.CaseDetails = null;
            _coreDataApiClientMock.Setup(x => x.SendQueryAsync<ResponseCaseInformationByUrn>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(fakedResponse);

            var coreDataApiClient = GetCoreDataApiClient();

            //Act
            var results = await coreDataApiClient.GetCaseInformationByUrnAsync(_fixture.Create<string>(), _fixture.Create<string>());

            //Assert
            results.Should().BeNull();
        }

        [Fact]
        public async Task CoreDataApiClient_GetCaseInformationByUrnAsync_ThrowsException_IsCaughtSuccessfully()
        {
            //Arrange
            _authenticatedGraphQlHttpRequestFactoryMock.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<GraphQLHttpRequest>()))
                .Returns(new AuthenticatedGraphQlHttpRequest(_fixture.Create<string>(), _fixture.Create<GraphQLHttpRequest>()));

            var fakedResponse = _fixture.Create<GraphQLResponse<ResponseCaseInformationByUrn>>();
            fakedResponse.Data.CaseDetails = _fixture.CreateMany<CaseDetails>(5).ToList();
            _coreDataApiClientMock.Setup(x => x.SendQueryAsync<ResponseCaseInformationByUrn>(It.IsAny<GraphQLRequest>(), It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            var coreDataApiClient = GetCoreDataApiClient();

            //Act
            var results = async () => await coreDataApiClient.GetCaseInformationByUrnAsync(_fixture.Create<string>(), _fixture.Create<string>());

            //Assert
            await results.Should().ThrowAsync<Exception>();
        }

        #region private methods

        private CoreDataApiClient GetCoreDataApiClient()
        {
            return new CoreDataApiClient(_coreDataApiClientMock.Object, _authenticatedGraphQlHttpRequestFactoryMock.Object);
        }

        #endregion private methods
    }

}
