using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

public partial class Chat
{
    public int Id { get; set; }

    public string ChatId { get; set; } = null!;

    public int User { get; set; }

    public virtual User IdNavigation { get; set; } = null!;
}
