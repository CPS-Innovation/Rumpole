using Microsoft.Extensions.Logging;
using NSubstitute;
using RumpoleGateway.Triggers.Status;
using Xunit;

namespace RumpoleGateway.Tests.Triggers.Status
{
	public class CoreDataApiCaseInformationByUrnFunctionTests : SharedMethods.SharedMethods
	{
		private readonly ILogger<StatusFunction> _mockLogger = Substitute.For<ILogger<StatusFunction>>();

		[Fact]
		public void StatusFunction_Should_Return_Response_401_When_No_Authorization_Supplied()
		{
			//Arrange
			var statusFunction = GetStatusFunction();

			//Act
			var results = statusFunction.Run(CreateHttpRequestWithoutToken()) as Microsoft.AspNetCore.Mvc.ObjectResult;

			//Assert
			Assert.Equal(401, results.StatusCode);
			Assert.Equal(Constants.CommonUserMessages.AuthenticationFailedMessage, results.Value);
		}

		[Fact]
		public void StatusFunction_Should_Return_Response_200()
		{
			//Arrange
			var statusFunction = GetStatusFunction();

			//Act
			var results = statusFunction.Run(CreateHttpRequest()) as Microsoft.AspNetCore.Mvc.ObjectResult;

			//Assert
			var response = results.Value as Domain.Status.Status;
			Assert.Equal(200, results.StatusCode);
		}


		#region private methods
		private StatusFunction GetStatusFunction()
		{
			return new StatusFunction(_mockLogger) { };
		}
		#endregion private methods

	}
}
