namespace AuthApi.Models.Dtos
{
    public class SigninDto
    {
        public string NameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
