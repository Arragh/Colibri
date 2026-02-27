namespace Colibri.Runtime.Pipeline.Cluster.CircuitBreaker;

public sealed class CircuitBreaker
{
    private readonly int _failures;
    private readonly int _timeout;
    private readonly HostState[] _hostsStates;

    public CircuitBreaker(int hostsCount, int failures, int timeout)
    {
        _failures = failures;
        _timeout = 1000 * timeout;
        
        _hostsStates = new HostState[hostsCount];
        for (var i = 0; i < _hostsStates.Length; i++)
        {
            _hostsStates[i] = new HostState();
        }
    }
    
    public bool CanExecute(int hostIdx)
    {
        var host = _hostsStates[hostIdx];
        var state = Volatile.Read(ref host.State);

        if (state == 0)
        {
            return true;
        }

        if (state == 1)
        {
            var now = Environment.TickCount64;
            var opened = Volatile.Read(ref host.OpenedAtTicks);

            if (now - opened >= _timeout)
            {
                var newValue = 2;
                var expectedValue = 1;
                var status = Interlocked.CompareExchange(
                    ref host.State,
                    newValue,
                    expectedValue) == 1;
                
                return status;
            }
        }
        
        return false;
    }

    public void ReportResult(int hostIdx, bool success)
    {
        var host = _hostsStates[hostIdx];

        if (success)
        {
            Volatile.Write(ref host.Failures, 0);
            Volatile.Write(ref host.State, 0);
        }
        
        var fails = Interlocked.Increment(ref host.Failures);
        if (fails >= _failures)
        {
            Interlocked.Exchange(ref host.OpenedAtTicks, Environment.TickCount64);
            Volatile.Write(ref host.State, 1);
        }
    }
}