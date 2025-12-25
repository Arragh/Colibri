namespace Colibri.Services.TransportPool.Interfaces;

/// <summary>
/// Хранилище всех транспортов по ключу
/// В будущем можно добавить метод получения других транспортов, например gRPC
/// </summary>
public interface ITransportPool
{
    HttpMessageInvoker GetHttpInvoker(string baseUrl);
}