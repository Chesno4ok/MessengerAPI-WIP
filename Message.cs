using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

public partial class Message
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ChatId { get; set; }

    public DateTime Date { get; set; }

    public short IsRead { get; set; }

    public string? Text { get; set; }

    public virtual Chat Chat { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
