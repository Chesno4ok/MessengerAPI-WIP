using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

[JsonObject(MemberSerialization.OptIn)]
public partial class Chat
{
    [JsonProperty]
    public int Id { get; set; }

    [JsonProperty]
    public string ChatName { get; set; } = null!;
    [JsonProperty]
    public virtual ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
