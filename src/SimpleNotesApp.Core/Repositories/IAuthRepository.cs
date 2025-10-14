using SimpleNotesApp.Core.Models;
using SimpleNotesApp.Core.Repositories.Requests;

namespace SimpleNotesApp.Core.Repositories;

public interface IAuthRepository
{
  Task<bool> UserExistsAsync(string email);
  Task<bool> RegisterUserAsync(RegisterUserRequest request);
  Task<UserForLoginConfirmation?> GetUserForLoginAsync(string email);
  Task<int?> GetUserIdByEmailAsync(string email);
  Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
  Task<bool> UpdateRefreshTokenAsync(UpdateRefreshTokenRequest request);
  Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
  Task<UserForPasswordChange?> GetUserForPasswordChangeAsync(int userId);
  Task<bool> UpsertUserForOtpAsync(UpsertUserForOtpRequest request);
}
