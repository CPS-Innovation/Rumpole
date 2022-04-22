using System;
namespace RumpoleGateway.Domain.CoreDataApi
{
	public class CoreDataApiException : Exception
	{
		public CoreDataApiException(string message, Exception innerException): base(message, innerException)
		{
		}
	}
}

