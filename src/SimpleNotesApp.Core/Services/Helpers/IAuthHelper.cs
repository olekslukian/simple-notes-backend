namespace SimpleNotesApp.Core.Services.Helpers;

public interface IAuthHelper
{
  byte[] GetPasswordHash(string password, byte[] passwordSalt);
  string CreateToken(int userId);
  string CreateRefreshToken();
}
