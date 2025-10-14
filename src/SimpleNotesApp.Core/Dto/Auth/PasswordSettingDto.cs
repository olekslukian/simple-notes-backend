namespace SimpleNotesApp.Core.Dto.Auth;

public partial class PasswordSettingDto
{
    public string Otp { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }

    public PasswordSettingDto()
    {
        Otp ??= string.Empty;
        Password ??= string.Empty;
        PasswordConfirmation ??= string.Empty;
    }
}
