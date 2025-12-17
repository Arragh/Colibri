namespace Core.Interfaces.Services.Http;

public interface IHttpTransportProvider
{
    IHttpTransport GetHttpTransport(string key);
}