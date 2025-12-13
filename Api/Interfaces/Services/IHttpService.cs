namespace Api.Interfaces.Services;

public interface IHttpService
{
    HttpMessageInvoker GetClient(string key);
}