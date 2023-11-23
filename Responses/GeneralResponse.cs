namespace ChesnokMessengerAPI.Responses
{
    public class Response
    {
        public Response(string status, string message)
        {
            this.status = status;
            this.message = message;
        }

        public string? status;
        public string? message;
    }
}
