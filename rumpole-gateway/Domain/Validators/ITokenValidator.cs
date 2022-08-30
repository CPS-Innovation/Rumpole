using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace RumpoleGateway.Domain.Validators
{
    public interface ITokenValidator
    {
        public Task<bool> ValidateTokenAsync(StringValues token);
    }
}
