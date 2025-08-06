using SimpleNotesApp.Infrastructure.Data;
using SimpleNotesApp.Infrastructure.Models;
using SimpleNotesApp.Infrastructure.Repositories.Requests;

namespace SimpleNotesApp.Infrastructure.Repositories;

public class AuthRepository(DbContext db) : IAuthRepository
{
  private readonly DbContext _db = db;
  public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
  {
    return await _db.QuerySingleAsync<User>(SPConstants.GET_USER_BY_REF_TOKEN, new { RefreshToken = refreshToken });
  }

  public async Task<UserForLoginConfirmation?> GetUserForLoginAsync(string email)
  {
    return await _db.QuerySingleAsync<UserForLoginConfirmation>(
           SPConstants.USER_AUTH_CONFIRMATION,
           new { Email = email }
       );
  }

  public async Task<int?> GetUserIdByEmailAsync(string email)
  {
    var result = await _db.QuerySingleAsync<int?>(SPConstants.USERID_GET, new { Email = email });
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

    return await _db.ExecuteAsync(SPConstants.REGISTRATION_UPSERT, parameters);
  }

  public async Task<bool> UpdateRefreshTokenAsync(UpdateRefreshTokenRequest request)
  {
    var parameters = new
    {
      UserId = request.UserId,
      RefreshToken = request.RefreshToken,
      RefreshTokenExpires = request.RefreshTokenExpires
    };

    return await _db.ExecuteAsync(SPConstants.REFRESH_TOKEN_UPDATE, parameters);
  }

  public async Task<bool> UserExistsAsync(string email)
  {
    var existingUsers = await _db.QueryAsync<string>(SPConstants.CHECK_USER, new { Email = email });
    return existingUsers.Any();
  }
}
