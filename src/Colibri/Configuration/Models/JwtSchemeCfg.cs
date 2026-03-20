namespace Colibri.Configuration.Models;

public class JwtSchemeCfg
{
    public string Name { get; set; }
    public string Issuer { get; set; }
    public bool ValidateIssuer { get; set; }
    public string Audience { get; set; }
    public bool ValidateAudience { get; set; }
    public string IssuerSigningKey { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public string Algorithm  { get; set; }
    public string Key { get; set; }
}