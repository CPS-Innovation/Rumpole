using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace RumpoleGateway.Functions.Status
{
	public class Status
	{
		private readonly ILogger<Status> _logger;
		public Status(ILogger<Status> logger)
		{
			_logger = logger;
		}

		[FunctionName("Status")]
		public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "status")] HttpRequest req)
		{
			var version = Assembly
				.GetExecutingAssembly()
				.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
				.InformationalVersion;

			if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
			{
				var errorMsg = "Authorization token is not supplied.";
				_logger.LogError(errorMsg);
				return new UnauthorizedObjectResult(errorMsg);
			}

			var response = new Domain.Status.Status
			{
				Message = $"Gateway - Version : {version}"
			};

			return new OkObjectResult(response);
		}
	}
}

