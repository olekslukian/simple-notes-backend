namespace SimpleNotesApp.DTO;
public partial class UserForRegistrationDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }

    public UserForRegistrationDTO()
    {
        Email ??= "";
        Password ??= "";
        PasswordConfirmation ??= "";
    }
}