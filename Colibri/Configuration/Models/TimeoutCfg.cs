namespace Colibri.Configuration.Models;

public sealed class TimeoutCfg
{
    public required int Request { get; set; }
    public required int Connect { get; set; }
}