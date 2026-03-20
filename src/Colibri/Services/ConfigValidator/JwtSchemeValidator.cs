using Colibri.Helpers;

namespace Colibri.Services.ConfigValidator;

public sealed class JwtSchemeValidator
{
    public bool AuthAlgorithmIsNotEmpty(string? authorizationType)
    {
        return !string.IsNullOrWhiteSpace(authorizationType);
    }

    public bool AuthAlgorithmIsValid(string authorizationType)
    {
        return GlobalConstants.AuthAlgorithms.Contains(authorizationType);
    }
}