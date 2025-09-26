using SimpleNotesApp.Core.Models;
using SimpleNotesApp.Core.Repositories;
using SimpleNotesApp.Core.Repositories.Requests;
using SimpleNotesApp.Infrastructure.Data;

namespace SimpleNotesApp.Infrastructure.Repositories;

public class AuthRepository(DbContext db) : IAuthRepository
{
  private readonly DbContext _db = db;
  public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
  {
    return await _db.QuerySingleAsync<User>(SP.GET_USER_BY_REF_TOKEN, new { RefreshToken = refreshToken });
  }

  public async Task<UserForLoginConfirmation?> GetUserForLoginAsync(string email)
  {
    return await _db.QuerySingleAsync<UserForLoginConfirmation>(
           SP.USER_AUTH_CONFIRMATION,
           new { Email = email }
       );
  }

  public async Task<int?> GetUserIdByEmailAsync(string email)
  {
    var result = await _db.QuerySingleAsync<int?>(SP.USERID_GET, new { Email = email });
    return result <= 0 ? null : result;
  }

  public async Task<bool> RegisterUserAsync(RegisterUserRequest request)
  {
    var parameters = new
    {
      Email = request.Email,
      PasswordHash = request.PasswordHash,
      PasswordSalt = request.PasswordSalt
    };

    return await _db.ExecuteAsync(SP.USER_REGISTER, parameters);
  }

  public async Task<bool> UpdateRefreshTokenAsync(UpdateRefreshTokenRequest request)
  {
    var parameters = new
    {
      UserId = request.UserId,
      RefreshToken = request.RefreshToken,
      RefreshTokenExpires = request.RefreshTokenExpires
    };

    return await _db.ExecuteAsync(SP.REFRESH_TOKEN_UPDATE, parameters);
  }

  public async Task<bool> UserExistsAsync(string email)
  {
    var existingUsers = await _db.QueryAsync<string>(SP.CHECK_USER, new { Email = email });
    return existingUsers.Any();
  }

  public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
  {
    var parameters = new
    {
      UserId = request.UserId,
      PasswordHash = request.PasswordHash,
      PasswordSalt = request.PasswordSalt
    };

    return await _db.ExecuteAsync(SP.PASSWORD_CHANGE, parameters);
  }

  public async Task<UserForPasswordChange?> GetUserForPasswordChangeAsync(int userId)
  {
    return await _db.QuerySingleAsync<UserForPasswordChange>(
      SP.GET_USER_FOR_PASSWORD_CHANGE,
      new { UserId = userId }
    );
  }
}
