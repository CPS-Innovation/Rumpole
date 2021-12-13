//using Microsoft.Graph;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;

//namespace RumpoleGateway.Factories.GraphServiceClientFactory
//{
//    public class GraphServiceClientFactory : IGraphServiceClientFactory
//    {
//        public GraphServiceClient CreateGraphServiceClient(string accessToken)
//        {
//            var authProvider = new DelegateAuthenticationProvider(
//            (requestMessage) =>
//            {
//                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//                return Task.FromResult(0);
//            });

//            return new GraphServiceClient(GraphClientFactory.Create(authProvider));
//        }
//    }
//}
