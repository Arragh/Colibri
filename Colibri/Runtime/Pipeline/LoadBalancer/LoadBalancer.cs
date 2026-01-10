namespace Colibri.Runtime.Pipeline.LoadBalancer;

public sealed class LoadBalancer
{
    private int _counter;

    public int SelectHost(int clusterId)
    {
        // return Interlocked.Increment(ref _counter) % 2;
        return 0; // Заглушка
    }
}