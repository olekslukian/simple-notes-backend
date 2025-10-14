namespace SimpleNotesApp.Core.Services.Helpers;

public interface IAuthHelper
{
  string GenerateOtp();
  bool IsValidEmail(string email);
  byte[] GetPasswordHash(string password, byte[] passwordSalt);
  string CreateToken(int userId);
  string CreateRefreshToken();
}
