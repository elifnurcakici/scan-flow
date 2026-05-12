using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace backend.Services.WebSockets
{
    public class WebSocketNotificationService
    {
        private readonly WebSocketConnectionManager _connectionManager;
        public WebSocketNotificationService(WebSocketConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task BroadcastAsync(object message)
        {
            var json = JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(json);

            foreach (var socket in _connectionManager.GetAllSockets())
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(
                        new ArraySegment<byte>(buffer),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
            }
        }
    }
}