namespace ChesnokMessengerAPI.Responses
{
    public partial class UserUpdateResponse
    {
        public int Id { get; set; }
        public int Updates { get; set; }
    }

    public partial class TokenResponse
    {
        public  int Id { get; set; }
        public  string? Token { get; set; }
    }

    public partial class UserResponse
    {
        
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
