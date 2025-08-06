using SimpleNotesApp.Models;
using SimpleNotesApp.Repositories.Requests;

namespace SimpleNotesApp.Repositories;

public interface IAuthRepository
{
  Task<bool> UserExistsAsync(string email);
  Task<bool> RegisterUserAsync(RegisterUserRequest request);
  Task<UserForLoginConfirmation?> GetUserForLoginAsync(string email);
  Task<int?> GetUserIdByEmailAsync(string email);
  Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
  Task<bool> UpdateRefreshTokenAsync(UpdateRefreshTokenRequest request);
}
