namespace Web.Model
{
    public class LoginRequestModel
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class OAuthLoginRequestModel
    {
        public string Code { get; set; }
    }
}
