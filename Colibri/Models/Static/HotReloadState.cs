using System.Text;

namespace Colibri.Models.Static;

internal static class HotReloadState
{
    public static int HotReloadCount = 0;
    public static readonly byte[] Server503Message = Encoding.UTF8.GetBytes("Server busy, hot reload in progress");
}