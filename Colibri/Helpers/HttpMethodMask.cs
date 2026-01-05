using Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

namespace Colibri.Helpers;

internal static class HttpMethodMask
{
    public static byte GetMask(string method)
    {
        switch (method.Length)
        {
            case 3: return method[0] == 'G'
                ? HttpMethodBits.Get
                : HttpMethodBits.Put;

            case 4: return method[0] == 'P'
                ? HttpMethodBits.Post
                : HttpMethodBits.Head;

            case 5: return HttpMethodBits.Patch;
            case 6: return HttpMethodBits.Delete;
            case 7: return HttpMethodBits.Options;
            
            default: return HttpMethodBits.None;
        }
    }
}