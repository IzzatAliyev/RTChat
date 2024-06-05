using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace RTChat.Api.Hubs;

public record User(string Name, string Room);
public record Message(string User, string Text);

public class ChatHub : Hub
{
    private static readonly ConcurrentDictionary<string, User> users = new();

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (users.TryGetValue(Context.ConnectionId, out var user))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.Room);
            await Clients.Group(user.Room).SendAsync("UserLeft", user.Name);
        }
    }
    
    public async Task JoinRoom(string userName, string roomName)
    {
        users.TryAdd(Context.ConnectionId, new User(userName, roomName));
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("UserJoined", userName);
    }
    
    public async Task SendMessageToRoom(string roomName, string content)
    {
        var message = new Message(users[Context.ConnectionId].Name, content);
        await Clients.Group(roomName).SendAsync("ReceiveMessage", message);
    }
}