namespace SimpleNotesApp.Dto.Auth;

public partial class UserForLoginConfirmationDto
{
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }

    public UserForLoginConfirmationDto()
    {
        PasswordHash = [];
        PasswordSalt = [];
    }
}
