using System;

namespace RumpoleGateway.Domain.Exceptions;

[Serializable]
public class CpsAuthenticationException: Exception
{
    public CpsAuthenticationException()
        : base("Invalid correlationId. A valid GUID is required.") { }
}
