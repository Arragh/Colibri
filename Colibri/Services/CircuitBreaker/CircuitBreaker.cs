using Colibri.Services.CircuitBreaker.Interfaces;

namespace Colibri.Services.CircuitBreaker;

public sealed class CircuitBreaker : ICircuitBreaker
{
    public bool CanExecute(int clusterId, int endpointId)
    {
        Console.WriteLine("Circuit Breaker Executed");
        
        return true;
    }

    public void ReportResult(int clusterId, int endpointId, bool success)
    {
        // throw  new NotImplementedException();
    }
}