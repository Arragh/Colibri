namespace Colibri.Configuration.Models;

public class JwtSchemeCfg
{
    public string Name { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public bool ValidateIssuer { get; set; }
    public string Audience { get; set; } = string.Empty;
    public bool ValidateAudience { get; set; }
    public string IssuerSigningKey { get; set; } = string.Empty;
    public bool ValidateIssuerSigningKey { get; set; }
    public string Algorithm  { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}