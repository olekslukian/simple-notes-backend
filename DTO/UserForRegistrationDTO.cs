namespace SimpleNotesApp.DTO;
public partial class UserForRegistrationDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }

    public UserForRegistrationDTO()
    {
        Email ??= string.Empty;
        Password ??= string.Empty;
        PasswordConfirmation ??= string.Empty;
    }
}