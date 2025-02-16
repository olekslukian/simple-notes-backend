namespace SimpleNotesApp.DTO;

public partial class UserForLoginConfirmationDTO
{
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }

    public UserForLoginConfirmationDTO()
    {
        PasswordHash = [];
        PasswordSalt = [];
    }
}
