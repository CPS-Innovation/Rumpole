//using Microsoft.Extensions.Options;
//using Microsoft.Identity.Client;
//using RumpoleGateway.Domain.Authorisation;
//using System;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading;
//using System.Threading.Tasks;

//namespace RumpoleGateway.Extensions
//{
//    public class AuthorizationHandler : DelegatingHandler
//    {
//        private readonly IOptions<AzureAdConfig> _config;

//        public AuthorizationHandler(IOptions<AzureAdConfig> config, HttpMessageHandler inner = null)
//            : base(inner ?? new HttpClientHandler())
//        {
//            _config = config;
//        }

//        protected override async Task<HttpResponseMessage> SendAsync(
//            HttpRequestMessage request,
//            CancellationToken cancellationToken)
//        {
//            var tempToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1yNS1BVWliZkJpaTdOZDFqQmViYXhib1hXMCIsImtpZCI6Ik1yNS1BVWliZkJpaTdOZDFqQmViYXhib1hXMCJ9.eyJhdWQiOiJhcGk6Ly81ZjFmNDMzYS00MWIzLTQ1ZDMtODk1ZS05MjdmNTAyMzJhNDciLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8wMGRkMGQxZC1kN2U2LTQzMzgtYWM1MS01NjUzMzljNzA4OGMvIiwiaWF0IjoxNjM5MzkxMjg2LCJuYmYiOjE2MzkzOTEyODYsImV4cCI6MTYzOTM5NTk4MiwiYWNyIjoiMSIsImFpbyI6IkUyWmdZQkIvOHFiTjlsem5zbk5Cb28wSi9uMldOWHVYbWMvVG1QT0doOCszeC82bml5WUEiLCJhbXIiOlsicHdkIl0sImFwcGlkIjoiNjM3YjliY2ItMzk1ZC00YmZjLWJlZGMtMzMwMmU1NzQ0ZTg0IiwiYXBwaWRhY3IiOiIwIiwiZmFtaWx5X25hbWUiOiJHb3ZpbmRhcmFqYW4iLCJnaXZlbl9uYW1lIjoiR29waWtyaXNobmEiLCJpcGFkZHIiOiI3OC4xNDYuMTg3LjUiLCJuYW1lIjoiR29waWtyaXNobmEgR292aW5kYXJhamFuIiwib2lkIjoiZjliNjBkZWYtMTNmNy00YzdjLThlMTctNmNjODczYTc1OWM2Iiwib25wcmVtX3NpZCI6IlMtMS01LTIxLTYwNjc0NzE0NS03OTY4NDU5NTctMTQxNzAwMTMzMy0zNzUxODYiLCJyaCI6IjAuQVNBQUhRM2RBT2JYT0VPc1VWWlRPY2NJak11YmUyTmRPZnhMdnR3ekF1VjBUb1FnQU93LiIsInNjcCI6ImNhc2UuY29uZmlybSIsInN1YiI6Ijg1Zi0tVGRGV2JxTUFWenR0YXJnZ0ZnbElta095MDRsbzY5dlBPU1VIVkEiLCJ0aWQiOiIwMGRkMGQxZC1kN2U2LTQzMzgtYWM1MS01NjUzMzljNzA4OGMiLCJ1bmlxdWVfbmFtZSI6IkdvcGkuR292aW5kYXJhamFuQGNwcy5nb3YudWsiLCJ1cG4iOiJHb3BpLkdvdmluZGFyYWphbkBjcHMuZ292LnVrIiwidXRpIjoiSENtbGhFcDhhRU9HMHBsU3F1bGhBQSIsInZlciI6IjEuMCJ9.DGzvCIrEZKFYgY5sSWdp_DOIQFuhezR-JF4FiC4q19aoRMTbccL8EKiG1MDNNoQcRElN7pQ1gMVb4SjwXpiJije9fX0x-olq5a__v5M0q8lLh0gDNIDvrwZp9qyzG9gtsIqwGQrNSc88vBMsvMAY1vDE4jeMAkekd3u4X9h81pYdZuZHBf90c6FcQ6wk2VY4L2ZBXfav0RkmwsETXVjVBkQ1S1iZPAJu8CrePsNI-cu-c5BB_23f3sD3ZVKtSfsZo-XJWsZZz_DCB5TARXKtbSdik75L6EyeIaS7KPf49ErfgMa4EuNfielKFU3sfpfln_zKs8zasGkNkV19um4B_A";
//            try
//            {
//                var app = ConfidentialClientApplicationBuilder.Create(_config.Value.ClientId)
//                    .WithClientSecret(_config.Value.ClientSecret)
//                    .WithAuthority(new Uri(_config.Value.Authority))
//                    .Build();

//                var token = await app.AcquireTokenForClient(_config.Value.Scopes)
//                    .ExecuteAsync(cancellationToken);
                
//               // request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
//                request.Headers.Authorization = new AuthenticationHeaderValue("bearer", tempToken);
//                return await base.SendAsync(request, cancellationToken);
//            }
//            catch(Exception ex)
//            {
//                var aa = ex.ToString();
//            }
//            return null;
//        }
//    }
//}
