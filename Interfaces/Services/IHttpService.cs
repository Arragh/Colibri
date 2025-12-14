namespace Interfaces.Services;

public interface IHttpService
{
    HttpMessageInvoker GetClient(string key);
}