using System.Text.RegularExpressions;

namespace Colibri.Services.ColibriConfiguration;

public sealed class GlobalValidator
{
    public RouteValidator Routes { get; } = new(SegmentOrPrefixNameIsValid);
    
    private static bool SegmentOrPrefixNameIsValid(string name)
    {
        var match = Regex.Match(name, "^[a-zA-Z0-9_]+$");
        return match.Success;
    }
}