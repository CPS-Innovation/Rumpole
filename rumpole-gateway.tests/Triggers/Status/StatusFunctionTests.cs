using Microsoft.Extensions.Logging;
using NSubstitute;
using RumpoleGateway.Triggers.Status;
using Xunit;

namespace RumpoleGateway.Tests.Triggers.Status
{
    public class StatusFunctionTests : SharedMethods.SharedMethods
    {
        private readonly ILogger<StatusFunction> _mockLogger = Substitute.For<ILogger<StatusFunction>>();

        [Fact]
        public void GetStatusFunction_Should_Return_Response_401_When_No_Authorization_Supplied()
        {
            //Arrange
            var statusFunction = GetStatusFunction();

            //Act
            var results = statusFunction.Run(CreateHttpRequestWithoutToken(), string.Empty) as Microsoft.AspNetCore.Mvc.ObjectResult;

            //Assert
            Assert.Equal(401, results.StatusCode);
            Assert.Equal(Constants.Status.Status.AuthenticationFailedMessage, results.Value);
        }

        [Fact]
        public void GetStatusFunction_Should_Return_Response_400_When_URN_Not_Supplied()
        {
            //Arrange
            var statusFunction = GetStatusFunction();

            //Act
            var results = statusFunction.Run(CreateHttpRequest(), string.Empty) as Microsoft.AspNetCore.Mvc.ObjectResult;

            //Assert
            Assert.Equal(400, results.StatusCode);
            Assert.Equal(Constants.Status.Status.URNNotSupplied, results.Value);
        }

        [Fact]
        public void GetStatusFunction_Should_Return_Response_200_When_Valid_Input_Supplied()
        {
            //Arrange
            var statusFunction = GetStatusFunction();
            var urn = "1234";

            //Act
            var results = statusFunction.Run(CreateHttpRequest(), urn) as Microsoft.AspNetCore.Mvc.ObjectResult;

            //Assert
            var response = results.Value as Domain.Status.Status;
            Assert.Equal(200, results.StatusCode);
            Assert.Equal(urn, response.URN);
        }


        #region private methods
        private StatusFunction GetStatusFunction()
        {
            return new StatusFunction(_mockLogger) { };
        }
        #endregion private methods

    }
}
