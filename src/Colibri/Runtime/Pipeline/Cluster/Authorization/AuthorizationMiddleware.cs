using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Colibri.Runtime.Pipeline.Cluster.Authorization;

public sealed class AuthorizationMiddleware(
    Authorizer[] authorizers,
    IMemoryCache cache) : IPipelineMiddleware
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

                if (validationResult.IsValid
                    && authorizer.TryAuthorize(validationResult.SecurityToken))
                {
                    authResult = true;
                    cachedSecurityToken = (JsonWebToken)validationResult.SecurityToken;
                    var ttl = validationResult.SecurityToken.ValidTo - DateTime.UtcNow;
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