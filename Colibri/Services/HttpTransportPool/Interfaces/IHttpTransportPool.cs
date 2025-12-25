namespace Colibri.Services.HttpTransportPool.Interfaces;

/// <summary>
/// Хранилище всех транспортов по ключу
/// В будущем можно добавить метод получения других транспортов, например gRPC
/// </summary>
public interface IHttpTransportPool
{
    HttpMessageInvoker GetHttpInvoker(string baseUrl);
}