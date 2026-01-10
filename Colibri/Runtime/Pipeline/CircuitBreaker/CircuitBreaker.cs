namespace Colibri.Runtime.Pipeline.CircuitBreaker;

public sealed class CircuitBreaker
{
    public bool CanExecute(int clusterId, int endpointId)
    {
        return true; // Заглушка
    }

    public void ReportResult(int clusterId, int endpointId, bool success)
    {
        // throw  new NotImplementedException(); // Заглушка
    }
}