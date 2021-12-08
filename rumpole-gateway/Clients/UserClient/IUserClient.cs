using RumpoleGateway.Domain.Authorisation;
using System.Threading.Tasks;

namespace RumpoleGateway.Clients.UserClient
{
    public interface IUserClient
    {
        Task<User> GetUser(string accessToken);
    }
}
