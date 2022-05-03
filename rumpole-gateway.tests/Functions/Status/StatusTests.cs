﻿using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace RumpoleGateway.Tests.Functions.Status
{
	public class CoreDataApiCaseInformationByUrnFunctionTests : SharedMethods.SharedMethods
	{
		private readonly ILogger<RumpoleGateway.Functions.Status.Status> _mockLogger = Substitute.For<ILogger<RumpoleGateway.Functions.Status.Status>>();

		[Fact]
		public void StatusFunction_Should_Return_Response_401_When_No_Authorization_Supplied()
		{
			//Arrange
			var statusFunction = GetStatusFunction();

			//Act
			var results = statusFunction.Run(CreateHttpRequestWithoutToken()) as Microsoft.AspNetCore.Mvc.ObjectResult;

			//Assert
			Assert.Equal(401, results.StatusCode);
		}

		[Fact]
		public void StatusFunction_Should_Return_Response_200()
		{
			//Arrange
			var statusFunction = GetStatusFunction();

			//Act
			var results = statusFunction.Run(CreateHttpRequest()) as Microsoft.AspNetCore.Mvc.ObjectResult;

			//Assert
			Assert.Equal(200, results.StatusCode);
		}

		private RumpoleGateway.Functions.Status.Status GetStatusFunction()
		{
			return new RumpoleGateway.Functions.Status.Status(_mockLogger);
		}

	}
}