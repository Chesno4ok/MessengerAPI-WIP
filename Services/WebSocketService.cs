using ChesnokMessengerAPI.WebSockets;
using Npgsql;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Permissions;
using System.Text.Json.Nodes;

namespace ChesnokMessengerAPI.Services
{
    public class WebSocketService
    {
        private List<WebSocketSession> webSocketSessions = new();
        public List<WebSocketSession> WebSocketSessions
        {
            get
            {
                object locker = new();
                lock (locker)
                {
                    return webSocketSessions;
                }
            }
            set
            {
                object locker = new();
                lock (locker)
                {
                    webSocketSessions = value;
                }
            }
        }
        private Timer clearSessionsTimer;
        public WebSocketService()
        {
            SubscribeToDbChanges();
        }
        public void OpenNewConnection(User user, WebSocket webSocket)
        {
            var webSocketSession = new WebSocketSession(webSocket, user);
            webSocketSession.CloseConnectionEvent += i => { i.Dispose(); WebSocketSessions.Remove(i); };
            
            WebSocketSessions.Add(webSocketSession);
        }

        private async void SubscribeToDbChanges()
        {
            using (var db = new NpgsqlConnection(DatabaseConnectionService.ConnectionString))
            {
                await db.OpenAsync();

                db.StateChange += Db_StateChange;

                db.Notification += OnDatabaseUpdate;

                await using (var cmd = new NpgsqlCommand("LISTEN datachange;", db))
                    cmd.ExecuteNonQuery();

                while (true)
                {
                    db.Wait();
                }
                
                await db.CloseAsync();
            }
        }

        private void Db_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            Console.WriteLine(e.CurrentState);
        }

        private void OnDatabaseUpdate(object sender, NpgsqlNotificationEventArgs e)
        {
            var dbNotification = System.Text.Json.JsonSerializer.Deserialize<DbNotification<Message>>(e.Payload);

            var jsonObj = JsonObject.Parse(e.Payload);

            using (var db = new MessengerContext())
            {
                var method = typeof(WebSocketService)
                    .GetMethods()
                    .Where(i => i.GetCustomAttribute<TableNotification>() != null)
                    .FirstOrDefault(i =>
                    i.GetCustomAttribute<TableNotification>().TableName == jsonObj["table"].ToString());

                if (method == null)
                    return;

                method.Invoke(this, new object[] {e.Payload});

            }
        }
        [TableNotification("Messages")]
        public void ProccessMessageUpdate(string payloadJson)
        {
            var webSocketSessions = WebSocketSessions;
            
            var message = System.Text.Json.JsonSerializer.Deserialize<DbNotification<Message>>(payloadJson);
            

            using var dbContext = new MessengerContext();

            var users = from p in dbContext.Users
                        join j in dbContext.ChatUsers on p.Id equals j.UserId
                        where j.ChatId == message.data.ChatId
                        select p;


            var sessions = from s in webSocketSessions
                           join u in users on s.User.Id equals u.Id
                           select s;
                            

            foreach(var i in sessions)
            {
                i.SendMessage(payloadJson);
            }
        }
        [TableNotification("Chats")]
        public void ProccessChatUpdate(string payloadJson)
        {
            //var message = System.Text.Json.JsonSerializer.Deserialize<DbNotification<Chat>>(payloadJson);
        }
        [TableNotification("ChatUsers")]
        public void ProccessChatUserUpdate(string payloadJson)
        {
            var chatUser = System.Text.Json.JsonSerializer.Deserialize<DbNotification<ChatUser>>(payloadJson);

            var sessions = WebSocketSessions.Where(i => i.User.Id == chatUser.data.UserId);

            foreach (var i in sessions)
            {
                i.SendMessage(payloadJson);
            }
        }
    }
    class TableNotification : Attribute
    {
        public string TableName;
        public string Action;
        public TableNotification(string tableName)
        {
            this.TableName = tableName;
            this.Action = "";
        }
        public TableNotification(string tableName, string action)
        {
            Action = action;
            TableName = tableName;
        }
    }
}
