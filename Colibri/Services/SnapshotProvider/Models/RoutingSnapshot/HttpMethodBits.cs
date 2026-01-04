namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public static class HttpMethodBits
{
    public const byte None    = 0;
    public const byte Get     = 1 << 0;
    public const byte Post    = 1 << 1;
    public const byte Put     = 1 << 2;
    public const byte Delete  = 1 << 3;
    public const byte Patch   = 1 << 4;
    public const byte Head    = 1 << 5;
    public const byte Options = 1 << 6;
}