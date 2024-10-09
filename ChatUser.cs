using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

[JsonObject(MemberSerialization.OptIn)]
public partial class ChatUser
{
    [JsonProperty]
    public int Id { get; set; }
    [JsonProperty]
    public int UserId { get; set; }
    [JsonProperty]
    public int ChatId { get; set; }

    public short HasUpdates { get; set; }

    public virtual Chat Chat { get; set; } = null!;
    [JsonProperty]
    public virtual User User { get; set; } = null!;
}
