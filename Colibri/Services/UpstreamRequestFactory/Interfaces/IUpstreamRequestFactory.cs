using Colibri.Services.UpstreamRequestFactory.Models.Abstract;

namespace Colibri.Services.UpstreamRequestFactory.Interfaces;

public interface IUpstreamRequestFactory
{
     UpstreamRequest FromHttpContext(HttpContext ctx);
}