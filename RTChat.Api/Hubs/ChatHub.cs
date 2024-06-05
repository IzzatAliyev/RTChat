using Microsoft.AspNetCore.SignalR;

namespace RTChat.Api.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string message)
    {
        await  Clients.All.SendAsync("ReceiveMessage", message);
    }
}