using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

public class ChatResponse
{
    public int Id { get; set; }

    public string ChatName { get; set; } = null!;

    public virtual ICollection<ChatUserResponse> ChatUsers { get; set; } = new List<ChatUserResponse>();

    public virtual ICollection<MessageResponse> Messages { get; set; } = new List<MessageResponse>();
}
