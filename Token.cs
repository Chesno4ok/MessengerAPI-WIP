using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

[JsonObject(MemberSerialization.OptIn)]
public partial class Token
{
    
    public int Id { get; set; }
    [JsonProperty]
    public string TokenHash { get; set; } = null!;
    [JsonProperty]
    public int UserId { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual User User { get; set; } = null!;
}
