namespace Colibri.Runtime.Pipeline.CircuitBreaker;

public sealed class CircuitBreaker
{
    private readonly byte _maxErrors = 3;
    private readonly int _timeout = 10_000;
    private readonly HostState[] _hostsStates;

    public CircuitBreaker(int hostsCount)
    {
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
        
        var errors = Interlocked.Increment(ref host.Failures);
        if (errors >= _maxErrors)
        {
            Interlocked.Exchange(ref host.OpenedAtTicks, Environment.TickCount64);
            Volatile.Write(ref host.State, 1);
        }
    }
}