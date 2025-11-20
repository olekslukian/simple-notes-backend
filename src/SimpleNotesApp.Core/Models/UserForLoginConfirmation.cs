namespace SimpleNotesApp.Core.Models;

public partial class UserForLoginConfirmation
{
  public int UserId { get; set; }
  public string Email { get; set; } = string.Empty;
  public byte[]? PasswordHash { get; set; } = null;
  public byte[]? PasswordSalt { get; set; } = null;
  public bool IsEmailVerified { get; set; }
}
