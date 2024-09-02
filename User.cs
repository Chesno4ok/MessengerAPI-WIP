using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

public partial class User
{
    public virtual int Id { get; set; }

    public virtual string Name { get; set; } = null!;

    public  string Login { get; set; } = null!;

    public  string Password { get; set; } = null!;

    public  string Token { get; set; } = null!;

    public  ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();

    public  ICollection<Message> Messages { get; set; } = new List<Message>();
}
public class UserInfo
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; } = null!;
}
public class UserCredentials
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; } = null!;
    public string Token { get; set; } = null!;
}