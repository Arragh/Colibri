namespace Colibri.Runtime.Pipeline.RoutingEngine;

public readonly struct ParamValue
{
    public readonly ushort Start;
    public readonly byte Length;

    public ParamValue(ushort start, byte length)
    {
        Start = start;
        Length = length;
    }
}