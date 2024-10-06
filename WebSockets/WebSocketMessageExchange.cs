using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using Azure;
using ChesnokMessengerAPI.Services;
using Newtonsoft;
using Newtonsoft.Json;
using Npgsql;
using NuGet.Protocol;

namespace ChesnokMessengerAPI.WebSockets
{
    public class WebSocketMessageExchange
    {
        private User user;
        private WebSocket webSocket;
        public WebSocketMessageExchange(WebSocket webSocket, User user)
        {
            this.user = user;
            this.webSocket = webSocket;

            Task.Run(() => ReceiveMessage());
            Task.Run(() => NotifyNewMessage());

            while (webSocket.State == WebSocketState.Open) { }
           
            webSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Connection has benn closed",
                CancellationToken.None);

            Console.WriteLine("Connection has closed!");

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
                    break;
                }


                messageJson = Encoding.UTF8.GetString(receiveBuffer);
                var message = ProccessNewMessage(messageJson);

                if (message == null)
                    continue;

                SaveNewMessage(message);

                Array.Clear(receiveBuffer);
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
                if (db.ChatUsers.FirstOrDefault(i => i.UserId == user.Id && i.ChatId == message.ChatId) == null)
                    return false;
            }

            return true;
        }
        private void SaveNewMessage(Message message)
        {
            message.Date = DateTime.UtcNow;
            message.UserId = user.Id;

            using (var db = new MessengerContext())
            {
                db.Messages.Add(message);
                try
                {
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
            }
        }
        private async Task NotifyNewMessage()
        {
            using (var db = new NpgsqlConnection(DatabaseConnectionService.ConnectionString))
            {
                await db.OpenAsync();

                db.Notification += OnNewMessage;

                await using (var cmd = new NpgsqlCommand("LISTEN datachange;", db))
                    cmd.ExecuteNonQuery();

                while (webSocket.State == WebSocketState.Open && db.State == System.Data.ConnectionState.Open)
                {
                    db.Wait();
                }

                await db.CloseAsync();
            }
        }
        private void OnNewMessage(object sender, NpgsqlNotificationEventArgs e)
        {
            var dbNotification = System.Text.Json.JsonSerializer.Deserialize<DbNotification<Message>>(e.Payload);

            using (var db = new MessengerContext())
            {
                //var chat = db.ChatUsers.FirstOrDefault(c => c.UserId == user.Id && c.ChatId == dbNotification.data.ChatId);

                //if (chat == null)
                //    return;

                SendMessage(e.Payload);
            }

        }
        private void SendMessage(string message)
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
                    return;
                }
            }


        }
    }
    public class TableNotification : Attribute
    {
        public string TableName;

        public TableNotification(string tableName)
        {
            TableName = tableName;
        }
    }
}
