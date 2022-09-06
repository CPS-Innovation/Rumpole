using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace RumpoleGateway.Domain.Validators
{
    public interface IJwtBearerValidator
    {
        public Task<bool> ValidateTokenAsync(StringValues token);
    }
}
