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
    public class CoreDataApiCaseDetailsFunctionTests : SharedMethods.SharedMethods , IClassFixture<CaseInformationFake>
    {
        private readonly ILogger<CoreDataApiCaseDetails> _mockLogger = Substitute.For<ILogger<CoreDataApiCaseDetails>>();
        private readonly IOnBehalfOfTokenClient _mockOnBehalfOfTokenClient = Substitute.For<IOnBehalfOfTokenClient>();
        private readonly ICoreDataApiClient _mockCoreDataApiClient = Substitute.For<ICoreDataApiClient>();
        private readonly CaseInformationFake _caseInformationFake;

        public CoreDataApiCaseDetailsFunctionTests(CaseInformationFake caseInformationFake)
        {
            _caseInformationFake = caseInformationFake; 
        }
        [Fact]
        public async Task CoreDataApiCaseDetailsFunction_Should_Return_Response_401_When_No_Authorization_Supplied()
        {
            //Arrange
            var coreDataApiCaseDetailsFunction = GetCoreDataApiCaseDetailsFunction();

            //Act
            var results = await coreDataApiCaseDetailsFunction.Run(CreateHttpRequestWithoutToken(), string.Empty) as Microsoft.AspNetCore.Mvc.ObjectResult;

            //Assert
            Assert.Equal(401, results.StatusCode);
        }

        [Fact]
        public async Task CoreDataApiCaseDetailsFunction_Should_Return_Response_400_When_Case_Id_Not_Supplied()
        {
            //Arrange
            var coreDataApiCaseDetailsFunction = GetCoreDataApiCaseDetailsFunction();

            //Act
            var results = await coreDataApiCaseDetailsFunction.Run(CreateHttpRequest(), string.Empty) as Microsoft.AspNetCore.Mvc.ObjectResult;

            //Assert
            Assert.Equal(400, results.StatusCode);
        }

        [Fact]
        public async Task CoreDataApiCaseDetailsFunction_Should_Return_Response_No_Data_Found()
        {
            //Arrange
            var caseId = "18846";
            var coreDataApiCaseDetailsFunction = GetCoreDataApiCaseDetailsFunction();

            //Act
            var results = await coreDataApiCaseDetailsFunction.Run(CreateHttpRequest(), caseId) as Microsoft.AspNetCore.Mvc.ObjectResult;

            //Assert
            Assert.Equal(404, results.StatusCode);
            Assert.Contains(caseId, results.Value.ToString());
             
        }


        [Fact]
        public async Task CoreDataApiCaseDetailsFunction_Should_Return_Response_200_When_Valid_Input_Supplied()
        {
            //Arrange
            var caseId = 18868;
            var coreDataApiCaseDetailsFunction = GetCoreDataApiCaseDetailsFunction();
            _mockCoreDataApiClient.GetCaseDetailsById(It.IsAny<string>(), It.IsAny<string>()).ReturnsForAnyArgs(_caseInformationFake.GetCaseInformationByURN_Payload()
                                                                                                                   .FirstOrDefault(x=>x.Id == caseId));

            //Act
            var results = await coreDataApiCaseDetailsFunction.Run(CreateHttpRequest(), caseId.ToString()) as Microsoft.AspNetCore.Mvc.ObjectResult;

            //Assert
            var response = results.Value as CaseDetails;
            Assert.Equal(200, results.StatusCode);
            Assert.Equal(caseId, response.Id);
        }

        private CoreDataApiCaseDetails GetCoreDataApiCaseDetailsFunction()
        {
            return new CoreDataApiCaseDetails(_mockLogger, _mockOnBehalfOfTokenClient, _mockCoreDataApiClient);
        }
    }
}
