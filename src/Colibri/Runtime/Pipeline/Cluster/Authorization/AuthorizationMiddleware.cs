using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Colibri.Runtime.Pipeline.Cluster.Authorization;

public sealed class AuthorizationMiddleware : IPipelineMiddleware
{
    private readonly JsonWebTokenHandler _handler;

    private readonly TokenValidationParameters _validationParameters;

    public AuthorizationMiddleware(string publicKey)
    {
        _handler = new JsonWebTokenHandler();
        
        var keyBytes = Convert.FromBase64String(publicKey);
        var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(keyBytes, out _);
        
        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }
    
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

        var result = await _handler.ValidateTokenAsync(token.ToString(), _validationParameters);

        if (!result.IsValid)
        {
            ctx.HttpContext.Response.StatusCode = 401;
            return;
        }
        
        await next(ctx);
    }
}