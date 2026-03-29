using System.Net;
using Colibri.Services;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Colibri.Runtime.Pipeline.Cluster.Authorization;

public sealed class AuthorizationMiddleware(
    Authorizer[] authorizers,
    TokenCache cache) : IPipelineMiddleware
{
    public async ValueTask InvokeAsync(PipelineContext ctx, PipelineDelegate next)
    {
        if (!ctx.HttpContext.Request.Headers
                .TryGetValue("Authorization", out var authHeader))
        {
            ctx.SetStatusCode(HttpStatusCode.Unauthorized);
            ctx.CommitStatusCode();
            return;
        }
        
        var authValue = authHeader.Count > 0 ? authHeader[0] : null;
        if (authValue == null
            || !authValue.StartsWith("Bearer ", StringComparison.Ordinal))
        {
            ctx.SetStatusCode(HttpStatusCode.Unauthorized);
            ctx.CommitStatusCode();
            return;
        }

        var token = authValue.AsSpan(7).ToString();
        bool authResult = false;

        if (cache.TryGetValue(token, out JsonWebToken? cachedSecurityToken))
        {
            foreach (var authorizer in authorizers)
            {
                if (authorizer.TryAuthorize(cachedSecurityToken!))
                {
                    authResult = true;
                    break;
                }
            }
        }
        else
        {
            foreach (var authorizer in authorizers)
            {
                var validationResult = await authorizer.ValidateToken(token);
                cachedSecurityToken = (JsonWebToken)validationResult.SecurityToken;

                if (validationResult.IsValid
                    && authorizer.TryAuthorize(cachedSecurityToken))
                {
                    authResult = true;
                    var ttl = validationResult.SecurityToken.ValidTo - DateTime.UtcNow;

                    if (ttl > TimeSpan.FromMinutes(2))
                    {
                        ttl = TimeSpan.FromMinutes(2);
                    }
                    
                    cache.Set(token, cachedSecurityToken, ttl);
                    break;
                }
            }
        }

        if (!authResult)
        {
            ctx.SetStatusCode(HttpStatusCode.Unauthorized);
            ctx.CommitStatusCode();
            return;
        }
        
        await next(ctx);
    }
}