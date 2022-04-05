using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace RumpoleGateway.Triggers.Status
{
	public class StatusFunction
	{
		private readonly ILogger<StatusFunction> _logger;
		public StatusFunction(ILogger<StatusFunction> logger)
		{
			_logger = logger;
		}

		[FunctionName("Status")]
		public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "status")] HttpRequest req)
		{
			_logger.LogInformation("Status function start");

			var version = Assembly
				.GetExecutingAssembly()
				.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
				.InformationalVersion;

			if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
			{
				return new UnauthorizedObjectResult(Constants.CommonUserMessages.AuthenticationFailedMessage);
			}

			var response = new Domain.Status.Status
			{
				Message = $"Gateway -  Version : {version}"
			};

			_logger.LogInformation($"Status function start");

			return new OkObjectResult(response);
		}
	}
}

