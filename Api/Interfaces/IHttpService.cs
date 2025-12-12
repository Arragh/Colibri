namespace Api.Interfaces;

public interface IHttpService
{
    HttpMessageInvoker GetClient(string key);
}