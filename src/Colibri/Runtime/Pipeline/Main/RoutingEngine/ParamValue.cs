namespace Colibri.Runtime.Pipeline.Main.RoutingEngine;

public readonly struct ParamValue(int start, ushort length)
{
    public readonly int Start = start;
    public readonly ushort Length = length;
}