namespace Colibri.Runtime.Pipeline.Cluster.Authorization;

public sealed class AuthorizationMiddleware(Authorizer authorizer) : IPipelineMiddleware
{
    public async ValueTask InvokeAsync(PipelineContext ctx, PipelineDelegate next)
    {
        if (!ctx.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            ctx.HttpContext.Response.StatusCode = 401;
            return;
        }
        
        var authValue = authHeader.Count > 0 ? authHeader[0] : null;

        if (authValue == null || !authValue.StartsWith("Bearer ", StringComparison.Ordinal))
        {
            ctx.HttpContext.Response.StatusCode = 401;
            return;
        }

        var token = authValue.AsSpan(7);
        
        var validationResult = await authorizer.ValidateToken(token.ToString());

        if (!validationResult.IsValid
            || !authorizer.TryAuthorize(validationResult.SecurityToken))
        {
            ctx.HttpContext.Response.StatusCode = 401;
            return;
        }
        
        await next(ctx);
    }
}