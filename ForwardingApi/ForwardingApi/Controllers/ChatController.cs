using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ForwardingApi.Controllers;

//[Route("api/chat")]
//[ApiController]
//public class ChatController : ControllerBase
//{
//    private readonly IHubContext<ChatHub> hubContext;
//    private readonly HubConnection _streamingHubConnection;

//    //public ChatController(IHubContext<ChatHub> hubContext)
//    //{
//    //    hubContext = hubContext;

//    //    // Create the connection to StreamingApi SignalR Hub
//    //    _streamingHubConnection = new HubConnectionBuilder()
//    //        .WithUrl("http://localhost:5042/chatHub")  // Ensure this matches your StreamingApi's URL and port
//    //        .WithAutomaticReconnect()
//    //        .Build();

//    //    // Set up the handler for receiving the message from StreamingApi
//    //    _streamingHubConnection.On<string>("ReceiveMessage", async (message) =>
//    //    {
//    //        // Relay the message to the MiddlewareApi clients
//    //        await hubContext.Clients.All.SendAsync("ReceiveMessage", message);
//    //    });

//    //    // Start the connection asynchronously
//    //    _ = _streamingHubConnection.StartAsync();
//    //}

//    // Endpoint to request the stream of messages from the StreamingApi
//    //[HttpPost("send")]
//    //public async Task<IActionResult> SendMessage()
//    //{
//    //    // Call StartStreaming method on the StreamingApi via SignalR
//    //    await _streamingHubConnection.SendAsync("StartStreaming");

//    //    // Return an immediate response, since streaming is now handled by SignalR
//    //    return Ok(new { Message = "Message sent to StreamingApi, streaming started." });
//    //}

//    public ChatController(IHubContext<ChatHub> hubContext)
//    {
//        hubContext = hubContext;

//        // Create a connection to the Streaming API SignalR Hub
//        _streamingHubConnection = new HubConnectionBuilder()
//            .WithUrl("wss://ai-api.governa.ai/api/v1/test/ws/8")
//            .WithAutomaticReconnect()
//            .Build();

//        // Relay messages from Streaming API to specific users in Middleware API
//        _streamingHubConnection.On<string>("ReceiveMessage", async (message) =>
//        {
//            await hubContext.Clients.All.SendAsync("ReceiveMessage", message);
//        });

//        _ = _streamingHubConnection.StartAsync();
//    }

//    [HttpPost("send")]
//    public async Task<IActionResult> SendMessage([FromForm] ChatRequestDto request)
//    {
//        // Call StartStreaming on the Streaming API via SignalR
//        await _streamingHubConnection.SendAsync("StartStreaming", request);

//        return Ok(new { Message = "Streaming started for user", UserId = request.UserId });
//    }
//}


[Route("api/chat")]
[ApiController]
public class ChatController(IWebSocketManagerService socketManager, IHubContext<ChatHub> hubContext) : ControllerBase
{
    [HttpPost("start/{userId}")]
    public async Task<IActionResult> Start([FromRoute] string userId)
    {
        // Connect to the WebSocket endpoint
        await socketManager.ConnectAsync(userId);

        // Start receiving messages from the WebSocket endpoint
        //await socketManager.ReceiveMessageAsync("StartResponse");

        return Ok(new { Message = "Connected to WebSocket endpoint" });
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromForm] ChatRequestDto request)
    {
        await socketManager.SendMessageAsync(request);

        await socketManager.ReceiveMessageAsync();

        return Ok(new { Message = "Message sent" });
    }
}
