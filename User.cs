using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int HasUpdates { get; set; }

    public string UserToken { get; set; } = null!;

    public virtual Chat? Chat { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
