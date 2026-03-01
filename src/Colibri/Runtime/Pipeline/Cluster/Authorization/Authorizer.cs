using System.Security.Cryptography;
using System.Text.Json;
using Colibri.Configuration.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Colibri.Runtime.Pipeline.Cluster.Authorization;

public sealed class Authorizer(ClaimCfg[] claims, string algorithm, string key)
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
    
    public Task<TokenValidationResult> ValidateToken(string token)
    {
        return _handler.ValidateTokenAsync(token, _validationParameters);
    }

    public bool TryAuthorize(SecurityToken securityToken)
    {
        var jwt = (JsonWebToken)securityToken;

        foreach (var claim in claims)
        {
            if (!jwt.TryGetPayloadValue(claim.Type, out object? element))
            {
                return false;
            }

            if (element is JsonElement jsonElement
                && jsonElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in jsonElement.EnumerateArray())
                {
                    foreach (var claimValue in claim.Value)
                    {
                        if (item.ValueEquals(claimValue))
                        {
                            return true;
                        }
                    }
                }
            }

            if (element is string elementString)
            {
                foreach (var claimValue in claim.Value)
                {
                    if (elementString.Equals(claimValue))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
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