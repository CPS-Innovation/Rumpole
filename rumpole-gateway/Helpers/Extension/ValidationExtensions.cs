using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RumpoleGateway.Domain.Validation;

namespace RumpoleGateway.Helpers.Extension
{
    public static class ValidationExtensions
    {
        public static BadRequestObjectResult ToBadRequest<T>(this ValidatableRequest<T> request)
        {
            return new BadRequestObjectResult(request.Errors.Select(e => new
            {
                Field = e.PropertyName,
                Error = e.ErrorMessage
            }));
        }
    }
}
