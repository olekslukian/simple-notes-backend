namespace SimpleNotesApp.Dto.Auth;

public partial class UserForRegistrationDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }

    public UserForRegistrationDto()
    {
        Email ??= string.Empty;
        Password ??= string.Empty;
        PasswordConfirmation ??= string.Empty;
    }
}
