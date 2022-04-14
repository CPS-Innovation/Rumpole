using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using RumpoleGateway.Clients.CoreDataApi;
using RumpoleGateway.Clients.OnBehalfOfTokenClient;
using RumpoleGateway.Domain.CoreDataApi.CaseDetails;
using RumpoleGateway.Tests.FakeData;
using RumpoleGateway.Functions.CoreDataApi;
using Xunit;

namespace RumpoleGateway.Tests.Functions.CoreDataApi
{
    public class CoreDataApiCaseInformationByUrnTests: SharedMethods.SharedMethods , IClassFixture<CaseInformationFake>
    {
        private readonly ILogger<CoreDataApiCaseInformationByUrn> _mockLogger = Substitute.For<ILogger<CoreDataApiCaseInformationByUrn>>();
        private readonly IOnBehalfOfTokenClient _mockOnBehalfOfTokenClient = Substitute.For<IOnBehalfOfTokenClient>();
        private readonly ICoreDataApiClient _mockCoreDataApiClient = Substitute.For<ICoreDataApiClient>();
        private readonly CaseInformationFake _caseInformationFake;

        public CoreDataApiCaseInformationByUrnTests(CaseInformationFake caseInformationFake)
        {
            _caseInformationFake = caseInformationFake; 
        }
        [Fact]
        public async Task CoreDataApiCaseInformationByUrnFunction_Should_Return_Response_401_When_No_Authorization_Supplied()
        {
            //Arrange
            var coreDataApiCaseInformationByUrnFunction = GetCoreDataApiCaseInformationByUrnFunction();

            //Act
            var results = await coreDataApiCaseInformationByUrnFunction.Run(CreateHttpRequestWithoutToken(), string.Empty) as Microsoft.AspNetCore.Mvc.ObjectResult;

            //Assert
            Assert.Equal(401, results.StatusCode);
        }

        [Fact]
        public async Task CoreDataApiCaseInformationByUrnFunction_Should_Return_Response_400_When_URN_Not_Supplied()
        {
            //Arrange
            var coreDataApiCaseInformationByUrnFunction = GetCoreDataApiCaseInformationByUrnFunction();

            //Act
            var results = await coreDataApiCaseInformationByUrnFunction.Run(CreateHttpRequest(), string.Empty) as Microsoft.AspNetCore.Mvc.ObjectResult;

            //Assert
            Assert.Equal(400, results.StatusCode);
        }

        [Fact]
        public async Task CoreDataApiCaseInformationByUrnFunction_Should_Return_Response_No_Data_Found()
        {
            //Arrange
            var urn = "10OF1234521";
            var coreDataApiCaseInformationByUrnFunction = GetCoreDataApiCaseInformationByUrnFunction();

            //Act
            var results = await coreDataApiCaseInformationByUrnFunction.Run(CreateHttpRequest(), urn) as Microsoft.AspNetCore.Mvc.ObjectResult;

            //Assert
            Assert.Equal(404, results.StatusCode);
            Assert.Contains(urn, results.Value.ToString());
             
        }


        [Fact]
        public async Task CoreDataApiCaseInformationByUrnFunction_Should_Return_Response_200_When_Valid_Input_Supplied()
        {
            //Arrange
            var urn = "10OF1234520";
            var coreDataApiCaseInformationByUrnFunction = GetCoreDataApiCaseInformationByUrnFunction();
            _mockCoreDataApiClient.GetCaseInformationByURN(It.IsAny<string>(), It.IsAny<string>()).ReturnsForAnyArgs(_caseInformationFake.GetCaseInformationByURN_Payload());

            //Act
            var results = await coreDataApiCaseInformationByUrnFunction.Run(CreateHttpRequest(), urn) as Microsoft.AspNetCore.Mvc.ObjectResult;

            //Assert
            var response = results.Value as List<CaseDetails>;
            Assert.Equal(200, results.StatusCode);
            Assert.True(response.Any());
            Assert.Equal(urn, response.FirstOrDefault().UniqueReferenceNumber);
        }

        private CoreDataApiCaseInformationByUrn GetCoreDataApiCaseInformationByUrnFunction()
        {
            return new CoreDataApiCaseInformationByUrn(_mockLogger, _mockOnBehalfOfTokenClient, _mockCoreDataApiClient);
        }
    }
}
