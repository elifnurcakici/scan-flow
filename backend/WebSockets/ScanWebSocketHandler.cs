using System.ComponentModel;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using backend.Services.WebSockets;

namespace backend.Services.WebSockets
{
    public class ScanWebSocketHandler
    {
        private readonly WebSocketConnectionManager _connectionManager;

        public ScanWebSocketHandler(WebSocketConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task HandleAsync(HttpContext context, WebSocket webSocket)
        {
            var socketId = _connectionManager.AddSocket(webSocket);
            var buffer = new byte[1024*4];

            try
            {
                while(webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        CancellationToken.None);

                    if (result.CloseStatus.HasValue)
                    {
                        await _connectionManager.RemoveSocketAsync(socketId);
                        break;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if(message == "ping")
                    {
                        var pong = Encoding.UTF8.GetBytes("pong");
                        await webSocket.SendAsync(
                            new ArraySegment<byte>(pong),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None);
                    }
                }
            }
            finally
            {
                await _connectionManager.RemoveSocketAsync(socketId);
            }
        }
    }
}