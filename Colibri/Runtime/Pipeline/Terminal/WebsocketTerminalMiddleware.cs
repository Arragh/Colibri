using System.Net.WebSockets;

namespace Colibri.Runtime.Pipeline.Terminal;

public class WebsocketTerminalMiddleware : IPipelineMiddleware
{
    private const string Protocol = "ws";
    private readonly Uri[] _uris;

    public WebsocketTerminalMiddleware(string[] hosts)
    {
        _uris = new Uri[hosts.Length];
        for (int i = 0; i < hosts.Length; i++)
        {
            _uris[i] = new Uri($"{Protocol}://{hosts[i]}");
        }
    }
    
    public async ValueTask InvokeAsync(PipelineContext ctx, PipelineDelegate next)
    {
        if (!ctx.HttpContext.WebSockets.IsWebSocketRequest)
        {
            ctx.HttpContext.Response.StatusCode = 400;
            return;
        }
        
        var requestUri = new Uri(
            _uris[ctx.HostIdx],
            ctx.DownstreamPath + ctx.HttpContext.Request.QueryString);
        
        var clientWebSocket = await ctx.HttpContext.WebSockets.AcceptWebSocketAsync();
        
        var backendWebSocket = new ClientWebSocket();
        await backendWebSocket.ConnectAsync(requestUri, ctx.CancellationToken);
        
        var clientToBackend = Task.Run(() => ProxyWebSocketDataAsync(clientWebSocket, backendWebSocket));
        var backendToClient = Task.Run(() => ProxyWebSocketDataAsync(backendWebSocket, clientWebSocket));
        
        await Task.WhenAny(clientToBackend, backendToClient);

        await CloseWebSocketAsync(clientWebSocket);
        await CloseWebSocketAsync(backendWebSocket);
    }
    
    private async Task ProxyWebSocketDataAsync(WebSocket client, WebSocket backend)
    {
        var buffer = new byte[1024 * 4];

        while (client.State is WebSocketState.Open
               && backend.State is WebSocketState.Open)
        {
            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            await SendAsync(backend, buffer, result.Count);
        }
    }
    
    private async Task SendAsync(WebSocket webSocket, byte[] buffer, int count)
    {
        await webSocket.SendAsync(
            buffer: new ArraySegment<byte>(buffer, 0, count),
            messageType: WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken: CancellationToken.None);
    }
    
    private async Task CloseWebSocketAsync(WebSocket webSocket)
    {
        try
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}