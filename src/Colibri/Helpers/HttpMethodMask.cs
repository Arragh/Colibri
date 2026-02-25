namespace Colibri.Helpers;

public static class HttpMethodMask
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
    
    private static class HttpMethodBits
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
}