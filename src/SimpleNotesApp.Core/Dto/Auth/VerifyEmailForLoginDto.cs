namespace SimpleNotesApp.Core.Dto.Auth;

public class VerifyEmailForLoginDto
{
  public string Email { get; set; }
  public string Otp { get; set; }

  public VerifyEmailForLoginDto()
  {
    Email ??= string.Empty;
    Otp ??= string.Empty;
  }
}


