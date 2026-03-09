namespace TelMedAPI.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordDTO
{
    public string PasswordActual { get; set; }
    public string PasswordNueva { get; set; }
}
}