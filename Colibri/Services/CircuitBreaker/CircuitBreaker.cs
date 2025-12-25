using System.Runtime.CompilerServices;
using Colibri.Services.CircuitBreaker.Interfaces;
using Colibri.Services.RoutingState.Models;

namespace Colibri.Services.CircuitBreaker;

/*
 * Реализация должна хранить состояние (closed, open, half-open) по каждому endpoint'у.
 */
public class CircuitBreaker : ICircuitBreaker
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanInvoke(ClusterConfig cluster, int endpointIndex)
    {
        /*
         * Проверяет, доступен ли выбранный эндпоинт для запросов.
         * Если да, то возвращает true, в противном случае false.
         */
        
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReportResult(ClusterConfig cluster, int endpointIndex, bool success)
    {
        /*
         * Получение результата о состоянии эндпоинта на основании ответа после выполнения запроса.
         * Нужно для формирования состояния доступности/недоступности определенных сервисов.
         */
        
        throw new NotImplementedException();
    }
}