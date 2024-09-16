using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;

namespace ChesnokMessengerAPI.WebSockets
{
    public class WebSocketSession
    {
        public string SessionID;
        public User User;
        private WebSocket webSocket;
        public delegate void CloseConnection(string SessionID);
        public event CloseConnection CloseConnectionEvent;

        public WebSocketSession(WebSocket webSocket, User user)
        {
            this.webSocket = webSocket;
            this.User = user;
            this.SessionID = System.Guid.NewGuid().ToString();
            StartExchange();
        }
        private async void StartExchange()
        {
            Task.Run(() => ReceiveMessage());
        }
        private async Task ReceiveMessage()
        {
            byte[]? receiveBuffer = new byte[1024 * 4];

            string messageJson = "";

            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var receiveResult = webSocket.ReceiveAsync(
                        receiveBuffer,
                        CancellationToken.None).Result;
                }
                catch
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.ProtocolError,
                        "Connection timed out",
                        CancellationToken.None);

                    CloseConnectionEvent.Invoke(SessionID);

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
            if (webSocket.State == WebSocketState.Open)
            {

                sendBuffer = Encoding.UTF8.GetBytes(message);

                try
                {
                    webSocket.SendAsync(sendBuffer,
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
                }
                catch
                {
                    webSocket.CloseAsync(
                        WebSocketCloseStatus.ProtocolError,
                        "Connection timed out",
                        CancellationToken.None);
                    CloseConnectionEvent.Invoke(SessionID);
                }
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
