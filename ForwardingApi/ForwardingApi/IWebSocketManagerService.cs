namespace ForwardingApi;

public interface IWebSocketManagerService
{
    Task ConnectAsync(string userId);
    Task SendMessageAsync(ChatRequestDto request);
    Task ReceiveMessageAsync();
}