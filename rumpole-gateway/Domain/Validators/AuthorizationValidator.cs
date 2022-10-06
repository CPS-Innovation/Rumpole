using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Helpers.Extension;

namespace RumpoleGateway.Domain.Validators
{
    [ExcludeFromCodeCoverage]
    public class AuthorizationValidator : IAuthorizationValidator
    {
        private readonly ILogger<AuthorizationValidator> _log;
        private const string ScopeType = @"http://schemas.microsoft.com/identity/claims/scope";
        private Guid _correlationId;

        public AuthorizationValidator(ILogger<AuthorizationValidator> log)
        {
            _log = log;
        }

        public async Task<bool> ValidateTokenAsync(StringValues token, Guid correlationId)
        {
            _log.LogMethodEntry(correlationId, nameof(ValidateTokenAsync), string.Empty);
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));
            _correlationId = correlationId;
            
            var issuer = $"https://sts.windows.net/{Environment.GetEnvironmentVariable("OnBehalfOfTokenTenantId")}/";
            var audience = Environment.GetEnvironmentVariable("CallingAppValidAudience");
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(issuer + "/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());

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
                
                var requiredScopes = Environment.GetEnvironmentVariable("CallingAppValidScopes")?.Replace(" ", string.Empty).Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).ToList();
                var requiredRoles = Environment.GetEnvironmentVariable("CallingAppValidRoles")?.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).ToList();
                
                return IsValid(claimsPrincipal, requiredScopes, requiredRoles);
            }
            catch (SecurityTokenValidationException securityException)
            {
                _log.LogMethodError(correlationId, nameof(ValidateTokenAsync), "A security exception was caught", securityException);
                return false;
            }
            catch (Exception ex)
            {
                _log.LogMethodError(correlationId, nameof(ValidateTokenAsync), "An unexpected error was caught", ex);
                return false;
            }
            finally
            {
                _log.LogMethodExit(correlationId, nameof(ValidateTokenAsync), string.Empty);
            }
        }

        private bool IsValid(ClaimsPrincipal claimsPrincipal, List<string> requiredScopes = null, List<string> requiredRoles = null)
        {
            _log.LogMethodEntry(_correlationId, nameof(IsValid), string.Empty);
            
            if (claimsPrincipal == null)
            {
                _log.LogMethodFlow(_correlationId, nameof(IsValid), "Claims Principal not found - returning 'false' indicating an authorization failure");
                return false;
            }

            requiredScopes = requiredScopes?.ToList() ?? new List<string>();
            requiredRoles = requiredRoles?.ToList() ?? new List<string>();

            if (!requiredScopes.Any() && !requiredRoles.Any())
            {
                _log.LogMethodFlow(_correlationId, nameof(IsValid), "No required scopes or roles found - allowing access - returning");
                return true;
            }

            var hasAccessToRoles = !requiredRoles.Any() || requiredRoles.All(claimsPrincipal.IsInRole);
            
            var scopeClaim = claimsPrincipal.HasClaim(x => x.Type == ScopeType)
                ? claimsPrincipal.Claims.First(x => x.Type == ScopeType).Value
                : string.Empty;

            var tokenScopes = scopeClaim.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var hasAccessToScopes = !requiredScopes.Any() || requiredScopes.All(x => tokenScopes.Any(y => string.Equals(x, y, StringComparison.OrdinalIgnoreCase)));

            _log.LogMethodExit(_correlationId, nameof(IsValid), $"Outcome role and scope checks - hasAccessToRoles: {hasAccessToRoles}, hasAccessToScopes: {hasAccessToScopes}");
            return hasAccessToRoles && hasAccessToScopes;
        }
    }
}
