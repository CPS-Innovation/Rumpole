using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using RumpoleGateway.Helpers.Extension;

namespace RumpoleGateway.Domain.Validators
{
    public class JwtBearerValidator : IJwtBearerValidator
    {
        private const string ScopeType = @"http://schemas.microsoft.com/identity/claims/scope";
        
        public async Task<bool> ValidateTokenAsync(StringValues token)
        {
            var issuer = $"https://sts.windows.net/{Environment.GetEnvironmentVariable("CallingAppTenantId")}/";
            var audience = Environment.GetEnvironmentVariable("CallingAppValidAudience");
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
                var tokenValidator = new JwtSecurityTokenHandler(); 
                var claimsPrincipal = tokenValidator.ValidateToken(token.ToJwtString(), validationParameters, out _);
                
                var requiredScopes = Environment.GetEnvironmentVariable("CallingAppValidScopes")?.Replace(" ", string.Empty)?.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)?.ToList();
                var requiredRoles = Environment.GetEnvironmentVariable("CallingAppValidRoles")?.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)?.ToList();
                
                return IsValid(claimsPrincipal, requiredScopes, requiredRoles);
            }
            catch (SecurityTokenValidationException)
            {
                return false;
            }
        }

        private static bool IsValid(ClaimsPrincipal claimsPrincipal, List<string> requiredScopes = null, List<string> requiredRoles = null)
        {
            if (claimsPrincipal == null)
            {
                return false;
            }

            requiredScopes = requiredScopes?.ToList() ?? new List<string>();
            requiredRoles = requiredRoles?.ToList() ?? new List<string>();

            if (!requiredScopes.Any() && !requiredRoles.Any())
            {
                return true;
            }

            var hasAccessToRoles = false;
            var hasAccessToScopes = false;

            hasAccessToRoles = !requiredRoles.Any() || requiredRoles.All(claimsPrincipal.IsInRole);

            if (!requiredScopes.Any()) return false;
            var scopeClaim = claimsPrincipal.HasClaim(x => x.Type == ScopeType)
                ? claimsPrincipal.Claims.First(x => x.Type == ScopeType).Value
                : string.Empty;

            var tokenScopes = scopeClaim.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)?.ToList() ?? new List<string>();
            hasAccessToScopes = requiredScopes.All(x => tokenScopes.Any(y => string.Equals(x, y, StringComparison.OrdinalIgnoreCase)));

            return hasAccessToRoles && hasAccessToScopes;
        }
    }
}
