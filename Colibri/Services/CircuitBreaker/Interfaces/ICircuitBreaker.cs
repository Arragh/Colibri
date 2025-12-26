namespace Colibri.Services.CircuitBreaker.Interfaces;

public interface ICircuitBreaker
{
    bool CanExecute(int clusterId, int endpointId);
    void ReportResult(int clusterId, int endpointId, bool success);
}