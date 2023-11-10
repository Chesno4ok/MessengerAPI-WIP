using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

public partial class Chat
{
    public int Id { get; set; }

    public int ChatId { get; set; }

    public int User { get; set; }

    public virtual User UserNavigation { get; set; } = null!;
}
