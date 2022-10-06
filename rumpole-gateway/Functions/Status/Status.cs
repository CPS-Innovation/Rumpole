using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Extensions;

namespace RumpoleGateway.Functions.Status
{
	public class Status : BaseRumpoleFunction
	{
		private readonly ILogger<Status> _logger;

		public Status(ILogger<Status> logger)
			: base(logger)
		{
			_logger = logger;
		}

		[FunctionName("Status")]
		public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "status")] HttpRequest req)
		{
			Guid currentCorrelationId = default;
			const string loggingName = "Status - Run";
			
			if (!req.Headers.TryGetValue("X-Correlation-ID", out var correlationId) ||
			    string.IsNullOrWhiteSpace(correlationId))
				return BadRequestErrorResponse("Invalid correlationId. A valid GUID is required.", currentCorrelationId, loggingName);

			if (!Guid.TryParse(correlationId, out currentCorrelationId))
				if (currentCorrelationId == Guid.Empty)
					return BadRequestErrorResponse("Invalid correlationId. A valid GUID is required.", currentCorrelationId, loggingName);

			_logger.LogMethodEntry(currentCorrelationId, loggingName, string.Empty);
			
			var version = Assembly
				.GetExecutingAssembly()
				.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;

			if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
			{
				return AuthorizationErrorResponse(currentCorrelationId, loggingName);
			}

			var response = new Domain.Status.Status
			{
				Message = $"Gateway - Version : {version}"
			};

			_logger.LogMethodExit(currentCorrelationId, loggingName, response.ToJson());
			return new OkObjectResult(response);
		}
	}
}

