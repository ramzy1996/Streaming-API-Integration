using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ForwardingApi;

public class WebSocketManagerService : IWebSocketManagerService
{
    private ClientWebSocket _webSocket;
    private readonly IHubContext<ChatHub> _hubContext;

    public WebSocketManagerService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
        _webSocket = new ClientWebSocket();
    }

    public async Task ConnectAsync(string userId)
    {
        await _webSocket.ConnectAsync(new Uri($"wss://ai-api.governa.ai/api/v1/test/ws/{userId}"), CancellationToken.None);
    }

    public async Task SendMessageAsync(ChatRequestDto request)
    {
        if (_webSocket.State != WebSocketState.Open)
        {
            if (request.UserId == Guid.Empty)
            {
                throw new Exception("User ID is not set");
            }

            await ConnectAsync(request.UserId.ToString());
        }

        // Send a message to the WebSocket endpoint
        var json = JsonSerializer.Serialize(request);
        var buffer = Encoding.UTF8.GetBytes(json);
        await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task ReceiveMessageAsync()
    {
        var receiveBuffer = new ArraySegment<byte>(new byte[1024]);
        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(receiveBuffer, CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                break;
            }

            var messageReceived = Encoding.UTF8.GetString(receiveBuffer.Array, 0, result.Count);

            if (string.IsNullOrWhiteSpace(messageReceived) || messageReceived == "\"\"")
            {
                continue;
            }

            try
            {
                var message = JsonSerializer.Deserialize<Message>(messageReceived, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (message.MessageType == "connection_ack")
                {
                    continue;
                }
            }
            catch (Exception)
            { }

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", messageReceived);
        }
    }
}