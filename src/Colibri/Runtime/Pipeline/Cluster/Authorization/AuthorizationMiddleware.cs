using System.Net;

namespace Colibri.Runtime.Pipeline.Cluster.Authorization;

public sealed class AuthorizationMiddleware(Authorizer authorizer) : IPipelineMiddleware
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

        var token = authValue.AsSpan(7);
        var validationResult = await authorizer.ValidateToken(token.ToString());

        if (!validationResult.IsValid
            || !authorizer.TryAuthorize(validationResult.SecurityToken))
        {
            ctx.SetStatusCode(HttpStatusCode.Unauthorized);
            ctx.CommitStatusCode();
            return;
        }
        
        await next(ctx);
    }
}