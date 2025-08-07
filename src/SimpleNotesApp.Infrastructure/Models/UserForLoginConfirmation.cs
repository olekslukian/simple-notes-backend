namespace SimpleNotesApp.Infrastructure.Models;

public partial class UserForLoginConfirmation
{
  public string Email { get; set; } = string.Empty;
  public byte[] PasswordHash { get; set; } = [];
  public byte[] PasswordSalt { get; set; } = [];
}
