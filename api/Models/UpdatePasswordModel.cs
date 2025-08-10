namespace npm.api.API.Models
{
    public class UpdatePasswordModel
    {
        public string Email { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}