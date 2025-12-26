using System.Runtime.CompilerServices;
using Colibri.Services.HttpTransportPool.Interfaces;

namespace Colibri.Services.HttpTransportPool;

public class HttpTransportPool : IHttpTransportPool
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HttpMessageInvoker GetHttpInvoker(string baseUrl)
    {
        /*
         * Хранит коллекцию транспортов с ключами по BaseUrl
         * Можно реализовать в стиле "GetOrCreate", когда, если нет транспорта, то создаем и отдаем.
         * После создания транспорта его обязательно где-то закэшировать по ключу "BaseUrl" для следующих запросов.
         */
        
        throw  new NotImplementedException();
    }
}