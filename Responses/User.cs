using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

public partial class UserResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}
public partial class UserCredentials
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Token { get; set; }
}
