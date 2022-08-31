using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client;
using RumpoleGateway.Helpers.Extension;

namespace RumpoleGateway.Domain.Validators
{
    public class TokenValidator : ITokenValidator
    {
        public async Task<bool> ValidateTokenAsync(StringValues token)
        {
            var issuer = $"https://sts.windows.net/{Environment.GetEnvironmentVariable("CallingAppTenantId")}/";
            var audience = Environment.GetEnvironmentVariable("CallingAppValidScopes");
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(issuer + "/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());

            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

            var discoveryDocument = await configurationManager.GetConfigurationAsync(default);
            var signingKeys = discoveryDocument.SigningKeys;

            var validationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = signingKeys,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2),
            };

            try
            {
                new JwtSecurityTokenHandler().ValidateToken(token.ToJwtString(), validationParameters, out _);
                return true;
            }
            catch (SecurityTokenValidationException)
            {
                return false;
            }
        }
    }
}
