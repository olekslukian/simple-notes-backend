namespace SimpleNotesApp.Core.Dto.Auth;

public partial class UserForLoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }

    public UserForLoginDto()
    {
        Email ??= string.Empty;
        Password ??= string.Empty;
    }
}
