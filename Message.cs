using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

public partial class Message
{
    public int Id { get; set; }

    public int FromUser { get; set; }

    public string ChatId { get; set; } = null!;

    public DateTime Date { get; set; }

    public string? Content { get; set; }

    public virtual User FromUserNavigation { get; set; } = null!;
}
