using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

public partial class Token
{
    public int Id { get; set; }

    public string TokenHash { get; set; } = null!;

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
