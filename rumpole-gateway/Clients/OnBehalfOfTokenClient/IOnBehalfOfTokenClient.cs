using System.Threading.Tasks;

namespace RumpoleGateway.Clients.OnBehalfOfTokenClient
{
    public interface IOnBehalfOfTokenClient
    {
        Task<string> GetAccessToken(string accessToken);
    }
}
