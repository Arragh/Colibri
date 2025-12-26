using System.Runtime.CompilerServices;
using Colibri.Services.UpstreamRequestFactory.Interfaces;
using Colibri.Services.UpstreamRequestFactory.Models.Abstract;

namespace Colibri.Services.UpstreamRequestFactory;

public class UpstreamRequestFactory : IUpstreamRequestFactory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UpstreamRequest FromHttpContext(HttpContext ctx)
    {
        /*
         * Создает экземпляр запроса для прохождения по всему пайплайну.
         */
        
        throw new NotImplementedException();
    }
}