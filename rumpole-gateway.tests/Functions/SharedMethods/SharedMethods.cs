using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace RumpoleGateway.Tests.Functions.SharedMethods
{

    public class SharedMethods
    {
        protected HttpRequest CreateHttpRequest()
        {
            const string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            var context = new DefaultHttpContext();
            context.Request.Headers.Add(new KeyValuePair<string, StringValues>(Constants.Authentication.Authorization, token));
            return context.Request;
        }

        protected HttpRequest CreateHttpRequestWithoutToken()
        {
            var context = new DefaultHttpContext();
            return context.Request;
        }
    }
}
