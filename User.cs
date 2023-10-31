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

    public string? UserToken { get; set; }
}
public partial class UserUpdateResponse
{
    public int Id { get; set; }
    public int HasUpdates { get; set; }
}

public partial class UserTokenResponse
{
    public int Id { get; set; }
    public string? UserToken { get; set; }
}