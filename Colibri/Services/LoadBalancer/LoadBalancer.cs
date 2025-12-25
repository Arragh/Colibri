using System.Runtime.CompilerServices;
using Colibri.Services.LoadBalancer.Interfaces;
using Colibri.Services.RoutingState.Models;

namespace Colibri.Services.LoadBalancer;

public class LoadBalancer : ILoadBalancer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Select(ClusterConfig clusterConfig)
    {
        /*
         * Балансировщик нагрузки, реализующий механизмы RoundRobin, Random и т.д.
         * Нужен для выбора из нескольких BaseUrl на кластер.
         */
        
        throw new NotImplementedException();
    }
}