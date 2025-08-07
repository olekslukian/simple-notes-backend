
namespace SimpleNotesApp.Core.Dto.Auth;

public partial class ChangePasswordDto
{
  public string OldPassword { get; set; }
  public string NewPassword { get; set; }
  public string NewPasswordConfirmation { get; set; }

  public ChangePasswordDto()
  {
    OldPassword ??= string.Empty;
    NewPassword ??= string.Empty;
    NewPasswordConfirmation ??= string.Empty;
  }
}
