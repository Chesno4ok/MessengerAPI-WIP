using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

public partial class Message
{
    public int Id { get; set; }

    public int FromUser { get; set; }

    public int ToUser { get; set; }

    public DateTime Date { get; set; }

    public string? Content { get; set; }
}
