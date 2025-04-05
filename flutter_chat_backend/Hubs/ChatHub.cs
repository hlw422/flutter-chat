using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace flutter_chat_backend.Hubs
{
    public class ChatHub : Hub
    {
        // 使用 ConcurrentDictionary 来存储所有连接的客户端
        private static readonly ConcurrentDictionary<string, string> ConnectedClients = new ConcurrentDictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            // 当客户端连接时，将其添加到 ConnectedClients 中
            ConnectedClients.TryAdd(Context.ConnectionId, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ?exception)
        {
            // 当客户端断开连接时，将其从 ConnectedClients 中移除
            ConnectedClients.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            string connectionId = Context.ConnectionId;
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // 获取所有连接的客户端
        public static IEnumerable<string> GetConnectedClients()
        {
            return ConnectedClients.Keys;
        }
    }
}