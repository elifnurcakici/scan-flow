using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace backend.Services.WebSockets
{
    public class WebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

        public string AddSocket(WebSocket socket)
        {
            var socketId = Guid.NewGuid().ToString();
            _sockets.TryAdd(socketId, socket);
            return socketId;
        }

        public async Task RemoveSocketAsync(string socketId)
        {
            if (_sockets.TryRemove(socketId, out var socket))
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Connection closed",
                        CancellationToken.None);
                }
            }
        }
        public IEnumerable<WebSocket> GetAllSockets()
        {
            return _sockets.Values;
        }
    }

}