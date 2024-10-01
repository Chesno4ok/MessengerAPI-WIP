using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace ChesnokMessengerAPI.WebSockets
{
    public class WebSocketSession : IDisposable
    {
        public User User;
        public WebSocket WebSocket;
        public delegate void CloseConnection(WebSocketSession session);
        public event CloseConnection CloseConnectionEvent;
        private CancellationTokenSource cancellationTokenSource = new();

        public WebSocketSession(WebSocket webSocket, User user)
        {
            this.WebSocket = webSocket;
            this.User = user;
            StartExchange();
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            WebSocket.Dispose();
        }

        private async void StartExchange()
        {
            CancellationToken ct = cancellationTokenSource.Token;
            Task.Run(() => ReceiveMessage(), ct);
        }
        private async Task ReceiveMessage()
        {
            byte[]? receiveBuffer = new byte[1024 * 4];

            string messageJson = "";

            while (WebSocket.State == WebSocketState.Open)
            {
                try
                {
                    CancellationToken ct = cancellationTokenSource.Token;
                    var receiveResult = WebSocket.ReceiveAsync(
                        receiveBuffer,
                        ct).Result;
                }
                catch
                {
                    CloseConnectionEvent.Invoke(this);

                    break;
                }


                messageJson = Encoding.UTF8.GetString(receiveBuffer).Trim('\0');
                var message = ProccessNewMessage(messageJson);
                
                if (message == null)
                    continue;

                SaveNewMessage(message);

                Array.Clear(receiveBuffer);
            }
        }

        public void SendMessage(string message)
        {
            byte[]? sendBuffer = new byte[1024 * 4];
            if (WebSocket.State == WebSocketState.Open)
            {

                sendBuffer = Encoding.UTF8.GetBytes(message);

                try
                {
                    WebSocket.SendAsync(sendBuffer,
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
                }
                catch
                {
                    WebSocket.CloseAsync(
                        WebSocketCloseStatus.ProtocolError,
                        "Connection timed out",
                        CancellationToken.None);
                    CloseConnectionEvent.Invoke(this);
                }
            }
            else
            {
                CloseConnectionEvent.Invoke(this);
            }
        }

        private Message? ProccessNewMessage(string messageJson)
        {
            Message? message = new Message();

            try
            {
                message = JsonConvert.DeserializeObject<Message>(messageJson);
            }
            catch
            {
                return null;
            }

            if (message == null)
                return null;

            if (!IsValidMessage(message))
                return null;

            return message;
        }

        private bool IsValidMessage(Message message)
        {
            using (var db = new MessengerContext())
            {
                if (db.Chats.FirstOrDefault(i => i.Id == message.ChatId) == null)
                    return false;
                if (db.ChatUsers.FirstOrDefault(i => i.UserId == User.Id && i.ChatId == message.ChatId) == null)
                    return false;
            }

            return true;
        }

        private void SaveNewMessage(Message message)
        {
            message.Date = DateTime.UtcNow;
            message.User = User.Id;

            using (var db = new MessengerContext())
            {
                db.Messages.Add(message);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }

       
    }
}
