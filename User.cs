using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

[JsonObject(MemberSerialization.OptIn)]
public partial class User
{
    [JsonProperty]
    public int Id { get; set; }
    [JsonProperty]
    public string Name { get; set; } = null!;

    public string LoginHash { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
}
