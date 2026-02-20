namespace Colibri.Runtime.Pipeline.RoutingEngine;

public readonly struct ParamValue
{
    public readonly int Start;
    public readonly ushort Length;

    public ParamValue(int start, ushort length)
    {
        Start = start;
        Length = length;
    }
}