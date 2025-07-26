using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using SimpleNotesApp.Data;
using SimpleNotesApp.Dto.Auth;
using SimpleNotesApp.Models;
using SimpleNotesApp.Services.Helpers;

namespace SimpleNotesApp.Services;

public class AuthService(DbContext db, IAuthHelper authHelper) : IAuthService
{
  private readonly DbContext _db = db;
  private readonly IAuthHelper _authHelper = authHelper;

  public ServiceResponse<bool> Register(UserForRegistrationDto user)
  {
    if (user.Password != user.PasswordConfirmation)
    {
      return ServiceResponse<bool>.Failure("Passwords do not match");
    }

    IEnumerable<string> existingUsers = _db.Query<string>(SPConstants.CHECK_USER, new { Email = user.Email });

    if (!existingUsers.IsNullOrEmpty())
    {
      return ServiceResponse<bool>.Failure("User already exists");
    }

    byte[] passwordSalt = new byte[16];

    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
    {
      rng.GetNonZeroBytes(passwordSalt);
    }

    byte[] passwordHash = _authHelper.GetPasswordHash(user.Password, passwordSalt);

    var registrationParams = new
    {
      Email = user.Email,
      PasswordHash = passwordHash,
      PasswordSalt = passwordSalt
    };

    bool isRegistered = _db.Execute(SPConstants.REGISTRATION_UPSERT, registrationParams);

    if (isRegistered)
    {
      return ServiceResponse<bool>.Success(true);
    }

    return ServiceResponse<bool>.Failure("Failed to register user");

  }
  public ServiceResponse<TokensResponseDto> Login(UserForLoginDto user)
  {
    var emailParam = new { Email = user.Email };

    UserForLoginConfirmationDto? userForConfirmation = _db.QuerySingle<UserForLoginConfirmationDto>(SPConstants.USER_AUTH_CONFIRMATION, emailParam);

    if (userForConfirmation == null)
    {
      return ServiceResponse<TokensResponseDto>.Failure("Email or password is incorrect");
    }

    if (userForConfirmation.PasswordSalt == null || userForConfirmation.PasswordHash == null)
    {
      return ServiceResponse<TokensResponseDto>.Failure("Email or password is incorrect");
    }

    byte[] passwordHash = _authHelper.GetPasswordHash(user.Password, userForConfirmation.PasswordSalt);

    if (passwordHash.Length != userForConfirmation.PasswordHash.Length)
    {
      return ServiceResponse<TokensResponseDto>.Failure("Email or password is incorrect");
    }

    bool isPasswordValid = true;
    for (int i = 0; i < passwordHash.Length; i++)
    {
      if (passwordHash[i] != userForConfirmation.PasswordHash[i])
      {
        isPasswordValid = false;
      }
    }

    if (!isPasswordValid)
    {
      return ServiceResponse<TokensResponseDto>.Failure("Email or password is incorrect");
    }

    int userId = _db.QuerySingle<int>(SPConstants.USERID_GET, emailParam);

    string refreshToken = CreateAndSaveRefreshToken(userId);

    if (refreshToken.IsNullOrEmpty())
    {
      return ServiceResponse<TokensResponseDto>.Failure("Something went wrong");
    }

    return ServiceResponse<TokensResponseDto>.Success(new(accessToken: _authHelper.CreateToken(userId), refreshToken: refreshToken));
  }
  public ServiceResponse<TokensResponseDto> RefreshToken(string refreshToken)
  {
    var tokenParam = new { RefreshToken = refreshToken };

    User? userFromDb = _db.QuerySingle<User>(SPConstants.GET_USER_BY_REF_TOKEN, tokenParam);

    if (userFromDb == null)
    {
      return ServiceResponse<TokensResponseDto>.Failure("Failed to refresh token");
    }

    if (refreshToken != userFromDb.RefreshToken)
    {
      return ServiceResponse<TokensResponseDto>.Failure("Refresh token is invalid");
    }

    if (userFromDb.RefreshTokenExpires.CompareTo(DateTime.UtcNow) < 0)
    {
      return ServiceResponse<TokensResponseDto>.Failure("Refresh token is expired");
    }

    string newAccessToken = _authHelper.CreateToken(userFromDb.UserId);
    string newRefreshToken = CreateAndSaveRefreshToken(userFromDb.UserId);

    return ServiceResponse<TokensResponseDto>.Success(new(accessToken: newAccessToken, refreshToken: newRefreshToken));
  }

  public ServiceResponse<string> TestAuth(string? userId)
  {
    if (string.IsNullOrEmpty(userId))
    {
      return ServiceResponse<string>.Failure("User is not authenticated");
    }

    return ServiceResponse<string>.Success(userId);
  }

  private string CreateAndSaveRefreshToken(int userId)
  {
    var expirationDate = DateTime.UtcNow.AddDays(7);

    var token = _authHelper.CreateRefreshToken();

    var tokenParams = new
    {
      UserId = userId,
      RefreshToken = token,
      RefreshTokenExpires = expirationDate
    };

    var result = _db.Execute(SPConstants.REFRESH_TOKEN_UPDATE, tokenParams);

    if (!result)
    {
      return "";
    }

    return token;
  }



}
