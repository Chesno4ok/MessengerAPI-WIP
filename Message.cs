using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

[JsonObject(MemberSerialization.OptIn)]
public partial class Message
{
    [JsonProperty]
    public int Id { get; set; }
    [JsonProperty]
    public int UserId { get; set; }
    [JsonProperty]
    public int ChatId { get; set; }
    [JsonProperty]
    public DateTime Date { get; set; }

    public short IsRead { get; set; }
    [JsonProperty]
    public string? Text { get; set; }

    public virtual Chat Chat { get; set; } = null!;
    [JsonProperty]
    public virtual User User { get; set; } = null!;
}
