namespace Colibri.Runtime.Pipeline.Cluster.CircuitBreaker;

public sealed class HostState
{
    public int State;
    public int Failures;
    public long OpenedAtTicks;
}