namespace ChesnokMessengerAPI.Responses
{
    public partial class UserUpdateResponse
    {
        public int Id { get; set; }
        public int HasUpdates { get; set; }
    }

    public partial class UserTokenResponse
    {
        public new int Id { get; set; }
        public new string? UserToken { get; set; }
    }

    public partial class UserResponse
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
    }
}
