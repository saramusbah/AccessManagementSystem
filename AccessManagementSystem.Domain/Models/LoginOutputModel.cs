namespace AccessManagementSystem.Domain.Models
{
    public class LoginOutputModel
    {
        public string Token { get; set; }

        public string UserEmail { get; set; }

        public static LoginOutputModel CreateForLogin(
            string token,
            string userEmail)
        {
            return new LoginOutputModel
            {
                Token = token,
                UserEmail = userEmail
            };
        }
    }
}