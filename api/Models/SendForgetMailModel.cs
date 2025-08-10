namespace npm.api.API.Models
{
    public class SendForgetMailModel
    {
        public string Email { get; set; }

        public string Secret { get; set; }

        public string AuthCode { get; set; }
    }
}