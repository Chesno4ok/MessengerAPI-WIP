using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI.Models;

public partial class File
{
    public int Id { get; set; }

    public int Message { get; set; }

    public string Type { get; set; } = null!;

    public byte[] Content { get; set; } = null!;
}
