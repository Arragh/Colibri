using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Colibri.Runtime.Pipeline.Cluster.Authorization;

public sealed class AuthorizationMiddleware(string algorithm, string key) : IPipelineMiddleware
{
    private readonly JsonWebTokenHandler _handler = new();

    private readonly TokenValidationParameters _validationParameters = new()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = GetSigningKey(algorithm, key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    
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

    private static SecurityKey GetSigningKey(string algorithm, string key)
    {
        var keyBytes = Convert.FromBase64String(key);
        
        if (algorithm == "rs256")
        {
            var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(keyBytes, out _);
            return new RsaSecurityKey(rsa);
        }
        
        if (algorithm == "hs256")
        {
            return new SymmetricSecurityKey(keyBytes);
        }

        var ecdsa = ECDsa.Create();
        ecdsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
        return new ECDsaSecurityKey(ecdsa);
    }
}