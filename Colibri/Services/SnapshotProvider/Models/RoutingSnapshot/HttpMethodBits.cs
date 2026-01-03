namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public static class HttpMethodBits
{
    public const int None    = 0;
    public const int Get     = 1 << 0;
    public const int Post    = 1 << 1;
    public const int Put     = 1 << 2;
    public const int Delete  = 1 << 3;
    public const int Patch   = 1 << 4;
    public const int Head    = 1 << 5;
    public const int Options = 1 << 6;
    public const int Trace   = 1 << 7;
    public const int Any     = -1;
}