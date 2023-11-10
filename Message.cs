using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

public partial class Message : MessageData
{
    public virtual User UserNavigation { get; set; } = null!;
}

public partial class MessageData
{
    public int Id { get; set; }

    public int User { get; set; }

    public int ChatId { get; set; }

    public DateTime Date { get; set; }

    public string? Content { get; set; }
}