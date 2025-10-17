



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


  public async Task<ServiceResponse<TokensResponseDto>> LoginWithPasswordAsync(UserForLoginDto user)
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

  public async Task<ServiceResponse<string>> SendOtpForLoginAsync(string email)
  {
    return await SendOtpByEmailAndSaveAsync(email);
  }

  public async Task<ServiceResponse<string>> SendOtpForPasswordSetAsync(int userId)
  {
    var email = await _repository.GetUserEmailByIdAsync(userId);

    if (string.IsNullOrEmpty(email))
    {
      return ServiceResponse<string>.Failure(Error.NotFound("Auth.UserNotFound", "User not found"));
    }

    return await SendOtpByEmailAndSaveAsync(email);
  }

  public async Task<ServiceResponse<bool>> SetUserPasswordAsync(int userId, PasswordSettingDto passwordDto)
  {
    if (userId <= 0)
    {
      return ServiceResponse<bool>.Failure(Error.Unauthorized("Auth.Unauthorized", "User not authorized"));
    }

    if (passwordDto.Password != passwordDto.PasswordConfirmation)
    {
      return ServiceResponse<bool>.Failure(Error.Validation("Auth.InvalidInput", "Passwords do not match"));
    }

    var email = await _repository.GetUserEmailByIdAsync(userId);

    var verificationResult = await VerifyEmailAndOtpAsync(email, passwordDto.Otp);

    if (!verificationResult.IsSuccess || verificationResult.Data == null)
      return ServiceResponse<bool>.Failure(verificationResult.Error ?? Error.Failure("Auth.VerificationFailed", "Email and OTP verification failed"));

    byte[] passwordSalt = new byte[16];

    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
    {
      rng.GetNonZeroBytes(passwordSalt);
    }

    byte[] passwordHash = _authHelper.GetPasswordHash(passwordDto.Password, passwordSalt);

    var request = new ChangePasswordRequest(
      UserId: userId,
      PasswordHash: passwordHash,
      PasswordSalt: passwordSalt
    );

    bool success = await _repository.ChangePasswordAsync(request);

    if (success)
    {
      return ServiceResponse<bool>.Success(true);
    }

    return ServiceResponse<bool>.Failure(Error.Failure("Auth.PasswordSetFailed", "Failed to set password"));
  }

  public async Task<ServiceResponse<TokensResponseDto>> VerifyEmailForLoginAsync(VerifyEmailForLoginDto verifyEmailForLoginDto)
  {
    var verificationResult = await VerifyEmailAndOtpAsync(verifyEmailForLoginDto.Email, verifyEmailForLoginDto.Otp);

    if (!verificationResult.IsSuccess || verificationResult.Data == null)
      return ServiceResponse<TokensResponseDto>.Failure(verificationResult.Error ?? Error.Failure("Auth.VerificationFailed", "Email and OTP verification failed"));

    var user = verificationResult.Data;

    string refreshToken = await CreateAndSaveRefreshTokenAsync(user.UserId);

    if (string.IsNullOrEmpty(refreshToken))
      return ServiceResponse<TokensResponseDto>.Failure(Error.Failure("Auth.TokenCreationFailed", "Something went wrong"));

    var tokens = new TokensResponseDto(
        accessToken: _authHelper.CreateToken(user.UserId),
        refreshToken: refreshToken
    );

    return ServiceResponse<TokensResponseDto>.Success(tokens);
  }

  private async Task<ServiceResponse<string>> SendOtpByEmailAndSaveAsync(string email)
  {
    if (!_authHelper.IsValidEmail(email))
    {
      return ServiceResponse<string>.Failure(Error.Validation("Auth.InvalidEmail", "Email format is invalid"));
    }

    var otp = _authHelper.GenerateOtp();

    byte[] otpSalt = new byte[16];

    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
    {
      rng.GetNonZeroBytes(otpSalt);
    }

    byte[] otpHash = _authHelper.GetPasswordHash(otp, otpSalt);

    var request = new UpsertUserForOtpRequest(
      Email: email,
      OtpHash: otpHash,
      OtpSalt: otpSalt,
      OtpExpiresAt: DateTime.UtcNow.AddMinutes(10)
    );

    bool dbResult = await _repository.UpsertUserForOtpAsync(request);

    if (!dbResult)
    {
      return ServiceResponse<string>.Failure(Error.Failure("Auth.OtpGenerationFailed", "Failed to generate OTP"));
    }

    var sendResult = await _emailService.SendVerificationEmailAsync(email, otp);

    return sendResult;
  }

  private async Task<ServiceResponse<UserForEmailConfirmation>> VerifyEmailAndOtpAsync(string? email, string otp)
  {
    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
      return ServiceResponse<UserForEmailConfirmation>.Failure(Error.Validation("Auth.InvalidInput", "Email and OTP are required"));

    if (!_authHelper.IsValidEmail(email))
      return ServiceResponse<UserForEmailConfirmation>.Failure(Error.Validation("Auth.InvalidEmail", "Email format is invalid"));

    var userForVerification = await _repository.GetUserForEmailConfirmationAsync(email);

    if (userForVerification == null)
      return ServiceResponse<UserForEmailConfirmation>.Failure(Error.NotFound("Auth.UserNotFound", "User not found"));

    if (userForVerification.OtpExpiresAt < DateTime.UtcNow)
      return ServiceResponse<UserForEmailConfirmation>.Failure(Error.Unauthorized("Auth.OtpExpired", "OTP has expired"));

    byte[] otpHash = _authHelper.GetPasswordHash(otp, userForVerification.OtpSalt);

    if (!IsPasswordValid(otpHash, userForVerification.OtpHash))
      return ServiceResponse<UserForEmailConfirmation>.Failure(Error.Unauthorized("Auth.InvalidCredentials", "Email or OTP is incorrect"));

    return ServiceResponse<UserForEmailConfirmation>.Success(userForVerification);
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


}
