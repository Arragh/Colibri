using System.Runtime.CompilerServices;
using Colibri.Services.RoutingEngine.Interfaces;
using Colibri.Services.RoutingEngine.Models;
using Colibri.Services.RoutingState.Interfaces;

namespace Colibri.Services.RoutingEngine;

public class RoutingEngine(IRoutingState routingState) : IRoutingEngine
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RoutingMatchResult? Match(ReadOnlySpan<char> path, HttpMethod method)
    {
        /*
         * В снапшоте RoutingState ищет по префиксу кластер и возвращает его, либо null
         */

        throw  new NotImplementedException();
    }
}