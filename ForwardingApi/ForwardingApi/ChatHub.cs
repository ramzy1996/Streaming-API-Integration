using Microsoft.AspNetCore.SignalR;

namespace ForwardingApi;

//public class ChatHub : Hub
//{
//    public async Task BroadcastMessage(string message)
//    {
//        await Clients.All.SendAsync("ReceiveMessage", message);
//    }
//}

//public class ChatHub : Hub
//{
//    // This will be used to relay messages to the clients connected to the MiddlewareApi
//}
public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"Client {Context.ConnectionId} connected");
    }

    //public override async Task OnDisconnectedAsync(Exception? exception)
    //{
    //    await base.OnDisconnectedAsync(exception);
    //    Console.WriteLine($"Client {Context.ConnectionId} disconnected");
    //}

    public async Task StartResponse(string message)
    {
        await Clients.All.SendAsync("StartResponse", message);
    }

    public async Task SendMessageResponse(string message)
    {
        await Clients.All.SendAsync("SendMessageResponse", message);
    }
}