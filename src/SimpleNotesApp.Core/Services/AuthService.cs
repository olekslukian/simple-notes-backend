



using System.Security.Cryptography;
using SimpleNotesApp.Core.Common;
using SimpleNotesApp.Core.Dto.Auth;
using SimpleNotesApp.Core.Models;
using SimpleNotesApp.Core.Repositories;
using SimpleNotesApp.Core.Repositories.Requests;
using SimpleNotesApp.Core.Services.Helpers;

namespace SimpleNotesApp.Core.Services;

public class AuthService(IAuthRepository repository, IEmailService emailService, IAuthHelper authHelper) : IAuthService
{
  private readonly IAuthRepository _repository = repository;
  private readonly IEmailService _emailService = emailService;
  private readonly IAuthHelper _authHelper = authHelper;


  public async Task<ServiceResponse<TokensResponseDto>> LoginAsync(UserForLoginDto user)
  {
    UserForLoginConfirmation? userForConfirmation = await _repository.GetUserForLoginAsync(user.Email);

    if (userForConfirmation == null)
    {
      return ServiceResponse<TokensResponseDto>.Failure(Error.Unauthorized("Auth.InvalidCredentials", "Email or password is incorrect"));
    }

    if (userForConfirmation.PasswordSalt == null || userForConfirmation.PasswordHash == null)
    {
      return ServiceResponse<TokensResponseDto>.Failure(Error.Unauthorized("Auth.InvalidCredentials", "Email or password is incorrect"));
    }

    byte[] passwordHash = _authHelper.GetPasswordHash(user.Password, userForConfirmation.PasswordSalt);

    if (!IsPasswordValid(passwordHash, userForConfirmation.PasswordHash))
    {
      return ServiceResponse<TokensResponseDto>.Failure(Error.Unauthorized("Auth.InvalidCredentials", "Email or password is incorrect"));
    }

    int? userId = await _repository.GetUserIdByEmailAsync(user.Email);

    if (userId == null)
    {
      return ServiceResponse<TokensResponseDto>.Failure(Error.Failure("Auth.UserNotFound", "Something went wrong"));
    }

    string refreshToken = await CreateAndSaveRefreshTokenAsync(userId.Value);

    if (string.IsNullOrEmpty(refreshToken))
    {
      return ServiceResponse<TokensResponseDto>.Failure(Error.Failure("Auth.TokenCreationFailed", "Something went wrong"));
    }

    var tokens = new TokensResponseDto(
        accessToken: _authHelper.CreateToken(userId.Value),
        refreshToken: refreshToken
    );

    return ServiceResponse<TokensResponseDto>.Success(tokens);
  }
  public async Task<ServiceResponse<TokensResponseDto>> RefreshTokenAsync(string refreshToken)
  {
    var userFromDb = await _repository.GetUserByRefreshTokenAsync(refreshToken);

    if (userFromDb == null)
    {
      return ServiceResponse<TokensResponseDto>.Failure(Error.Unauthorized("Auth.InvalidToken", "Refresh token is invalid"));
    }

    if (refreshToken != userFromDb.RefreshToken)
    {
      return ServiceResponse<TokensResponseDto>.Failure(Error.Unauthorized("Auth.InvalidToken", "Refresh token is invalid"));
    }

    if (userFromDb.RefreshTokenExpires.CompareTo(DateTime.UtcNow) < 0)
    {
      return ServiceResponse<TokensResponseDto>.Failure(Error.Unauthorized("Auth.TokenExpired", "Refresh token is expired"));
    }

    string newAccessToken = _authHelper.CreateToken(userFromDb.UserId);
    string newRefreshToken = await CreateAndSaveRefreshTokenAsync(userFromDb.UserId);

    var tokens = new TokensResponseDto(accessToken: newAccessToken, refreshToken: newRefreshToken);

    return ServiceResponse<TokensResponseDto>.Success(tokens);
  }

  public async Task<ServiceResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
  {
    if (userId <= 0)
    {
      return ServiceResponse<bool>.Failure(Error.Unauthorized("Auth.Unauthorized", "User not authorized"));
    }

    UserForPasswordChange? user = await _repository.GetUserForPasswordChangeAsync(userId);

    if (user == null)
    {
      return ServiceResponse<bool>.Failure(Error.NotFound("Auth.UserNotFound", "User not found"));
    }

    if (!IsPasswordValid(
        _authHelper.GetPasswordHash(changePasswordDto.OldPassword, user.PasswordSalt),
        user.PasswordHash))
    {
      return ServiceResponse<bool>.Failure(Error.Unauthorized("Auth.InvalidCredentials", "Current password is incorrect"));
    }

    if (changePasswordDto.NewPassword == changePasswordDto.OldPassword)
    {
      return ServiceResponse<bool>.Failure(Error.Validation("Auth.InvalidInput", "New password cannot be the same as the old password"));
    }

    if (changePasswordDto.NewPassword != changePasswordDto.NewPasswordConfirmation)
    {
      return ServiceResponse<bool>.Failure(Error.Validation("Auth.InvalidInput", "New passwords do not match"));
    }

    byte[] passwordSalt = new byte[16];

    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
    {
      rng.GetNonZeroBytes(passwordSalt);
    }

    byte[] passwordHash = _authHelper.GetPasswordHash(changePasswordDto.NewPassword, passwordSalt);

    var request = new ChangePasswordRequest(
      UserId: userId,
      PasswordHash: passwordHash,
      PasswordSalt: passwordSalt
    );

    bool success = await _repository.ChangePasswordAsync(request);

    Console.WriteLine($"Change password result: {success}");

    if (success)
    {
      return ServiceResponse<bool>.Success(true);
    }

    return ServiceResponse<bool>.Failure(Error.Failure("Auth.PasswordChangeFailed", "Failed to change password"));
  }

  private async Task<string> CreateAndSaveRefreshTokenAsync(int userId)
  {
    var expirationDate = DateTime.UtcNow.AddDays(7);

    var token = _authHelper.CreateRefreshToken();

    var request = new UpdateRefreshTokenRequest(userId, token, expirationDate);

    var result = await _repository.UpdateRefreshTokenAsync(request);

    if (!result)
    {
      return "";
    }

    return token;
  }

  private static bool IsPasswordValid(byte[] inputHash, byte[] storedHash)
  {
    if (inputHash.Length != storedHash.Length)
    {
      return false;
    }

    for (int i = 0; i < inputHash.Length; i++)
    {
      if (inputHash[i] != storedHash[i])
      {
        return false;
      }
    }

    return true;
  }

  public Task<ServiceResponse<bool>> SetUserPasswordAsync(int userId, PasswordSettingDto passwordDto)
  {
    throw new NotImplementedException();
  }

  public async Task<ServiceResponse<bool>> SendOtpForEmailVerificationAsync(string email)
  {
    if (!_authHelper.IsValidEmail(email))
    {
      return ServiceResponse<bool>.Failure(Error.Validation("Auth.InvalidEmail", "Email format is invalid"));
    }

    var otp = _authHelper.GenerateOtp();

    var sendResult = await _emailService.SendVerificationEmailAsync(email, otp);

    return sendResult;
  }
}
