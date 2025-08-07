using SimpleNotesApp.Infrastructure.Repositories.Requests;
using SimpleNotesApp.Infrastructure.Models;

namespace SimpleNotesApp.Infrastructure.Repositories;

public interface IAuthRepository
{
  Task<bool> UserExistsAsync(string email);
  Task<bool> RegisterUserAsync(RegisterUserRequest request);
  Task<UserForLoginConfirmation?> GetUserForLoginAsync(string email);
  Task<int?> GetUserIdByEmailAsync(string email);
  Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
  Task<bool> UpdateRefreshTokenAsync(UpdateRefreshTokenRequest request);
  Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
  Task<UserForPasswordChange?> GetUserForPasswordChangeAsync(string userId);
}
