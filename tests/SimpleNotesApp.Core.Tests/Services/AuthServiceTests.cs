using FluentAssertions;
using Moq;
using SimpleNotesApp.Core.Common;
using SimpleNotesApp.Core.Dto.Auth;
using SimpleNotesApp.Core.Models;
using SimpleNotesApp.Core.Repositories;
using SimpleNotesApp.Core.Repositories.Requests;
using SimpleNotesApp.Core.Services;
using SimpleNotesApp.Core.Services.Helpers;

namespace SimpleNotesApp.Core.Tests.Services;

public class AuthServiceTests
{
  private readonly Mock<IAuthRepository> _authRepository;
  private readonly Mock<IEmailService> _emailService;
  private readonly Mock<IAuthHelper> _authHelper;
  private readonly AuthService _authService;

  public AuthServiceTests()
  {
    _authRepository = new Mock<IAuthRepository>();
    _emailService = new Mock<IEmailService>();
    _authHelper = new Mock<IAuthHelper>();
    _authService = new AuthService(_authRepository.Object, _emailService.Object, _authHelper.Object);
  }

  #region LoginWithPasswordAsync Tests

  [Fact]
  public async Task LoginWithPasswordAsync_ValidCredentials_ReturnsToken()
  {
    var loginDto = new UserForLoginDto
    {
      Email = "test@example.com",
      Password = "password123"
    };

    var passwordSalt = new byte[16];
    var passwordHash = new byte[32];

    var userForConfirmation = new UserForLoginConfirmation
    {
      UserId = 1,
      Email = loginDto.Email,
      PasswordSalt = passwordSalt,
      PasswordHash = passwordHash,
      IsEmailVerified = true
    };

    _authRepository.Setup(r => r.GetUserForLoginAsync(loginDto.Email))
      .ReturnsAsync(userForConfirmation);

    _authHelper.Setup(h => h.GetPasswordHash(loginDto.Password, passwordSalt))
      .Returns(passwordHash);

    _authHelper.Setup(h => h.CreateToken(1))
      .Returns("test_valid_token");

    _authHelper.Setup(h => h.CreateRefreshToken())
      .Returns("test_valid_refresh_token");

    _authRepository.Setup(r => r.UpdateRefreshTokenAsync(It.IsAny<UpdateRefreshTokenRequest>()))
      .ReturnsAsync(true);

    var result = await _authService.LoginWithPasswordAsync(loginDto);

    result.IsSuccess.Should().BeTrue();
    result.Data.Should().NotBeNull();
    result.Data!.AccessToken.Should().Be("test_valid_token");
    result.Data.RefreshToken.Should().Be("test_valid_refresh_token");
  }

  [Fact]
  public async Task LoginWithPasswordAsync_UserNotFound_ReturnsUnauthorizedError()
  {
    var loginDto = new UserForLoginDto
    {
      Email = "test@example.com",
      Password = "password123"
    };

    _authRepository.Setup(r => r.GetUserForLoginAsync(loginDto.Email))
      .ReturnsAsync((UserForLoginConfirmation?)null);

    var result = await _authService.LoginWithPasswordAsync(loginDto);

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().NotBeNull();
    result.Error!.Code.Should().Be("Auth.InvalidCredentials");
  }

  [Fact]
  public async Task LoginWithPasswordAsync_UserWithNoPassword_ReturnsUnauthorizedError()
  {
    var loginDto = new UserForLoginDto
    {
      Email = "test@example.com",
      Password = "password123"
    };

    var userForConfirmation = new UserForLoginConfirmation
    {
      UserId = 1,
      Email = loginDto.Email,
      PasswordSalt = null,
      PasswordHash = null,
    };

    _authRepository.Setup(r => r.GetUserForLoginAsync(loginDto.Email))
      .ReturnsAsync(userForConfirmation);

    var result = await _authService.LoginWithPasswordAsync(loginDto);

    result.IsSuccess.Should().BeFalse();
    result.Error!.Code.Should().Be("Auth.InvalidCredentials");
  }

  [Fact]
  public async Task LoginWithPasswordAsync_EmailNotVerified_ReturnsUnauthorizedError()
  {
    var loginDto = new UserForLoginDto
    {
      Email = "test@example.com",
      Password = "password123"
    };

    var passwordSalt = new byte[16];
    var passwordHash = new byte[32];

    var userForConfirmation = new UserForLoginConfirmation
    {
      UserId = 1,
      Email = loginDto.Email,
      PasswordSalt = passwordSalt,
      PasswordHash = passwordHash,
      IsEmailVerified = false
    };

    _authRepository.Setup(r => r.GetUserForLoginAsync(loginDto.Email))
      .ReturnsAsync(userForConfirmation);


    var result = await _authService.LoginWithPasswordAsync(loginDto);

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().NotBeNull();
    result.Error!.Code.Should().Be("Auth.InvalidCredentials");
  }

  [Fact]
  public async Task LoginWithPasswordAsync_IncorrectPasswoed_ReturnsUnauthorizedError()
  {
    var loginDto = new UserForLoginDto
    {
      Email = "test@example.com",
      Password = "wrongpassword"
    };

    var passwordSalt = new byte[16];
    var correctPasswordHash = new byte[32];
    var wrongPasswordHash = new byte[32];

    wrongPasswordHash[0] = 1;

    var userForConfirmation = new UserForLoginConfirmation
    {
      UserId = 1,
      Email = loginDto.Email,
      PasswordSalt = passwordSalt,
      PasswordHash = correctPasswordHash,
      IsEmailVerified = true
    };

    _authRepository.Setup(r => r.GetUserForLoginAsync(loginDto.Email))
      .ReturnsAsync(userForConfirmation);

    _authHelper.Setup(h => h.GetPasswordHash(loginDto.Password, passwordSalt))
      .Returns(wrongPasswordHash);

    var result = await _authService.LoginWithPasswordAsync(loginDto);

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().NotBeNull();
    result.Error!.Type.Should().Be(ErrorType.Unauthorized);
    result.Error!.Code.Should().Be("Auth.InvalidCredentials");
  }

  #endregion

  #region SendOtpForLoginAsync Tests

  [Fact]
  public async Task SendOtpForLoginAsync_ValidEmail_SendsOtpAndReturnsObscuredEmail()
  {
    var email = "test@example.com";
    var otp = "123456";
    var obscuredEmail = "t**t@example.com";

    _authHelper.Setup(h => h.IsValidEmail(email)).Returns(true);
    _authHelper.Setup(h => h.GenerateOtp()).Returns(otp);
    _authHelper.Setup(h => h.GetPasswordHash(otp, It.IsAny<byte[]>())).Returns(new byte[32]);
    _authRepository.Setup(r => r.UpsertUserForOtpAsync(It.IsAny<UpsertUserForOtpRequest>()))
      .ReturnsAsync(true);
    _emailService.Setup(s => s.SendVerificationEmailAsync(email, otp))
      .ReturnsAsync(ServiceResponse<string>.Success(obscuredEmail));

    var result = await _authService.SendOtpForLoginAsync(email);

    result.IsSuccess.Should().BeTrue();
    result.Data.Should().Be(obscuredEmail);
    _emailService.Verify(s => s.SendVerificationEmailAsync(email, otp), Times.Once);
  }

  [Fact]
  public async Task SendOtpForLoginAsync_InvalidEmail_ReturnsValidationError()
  {
    var invalidEmail = "not-an-email";

    _authHelper.Setup(h => h.IsValidEmail(invalidEmail)).Returns(false);

    var result = await _authService.SendOtpForLoginAsync(invalidEmail);

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().NotBeNull();
    result.Error!.Type.Should().Be(ErrorType.Validation);
    _authRepository.Verify(r => r.UpsertUserForOtpAsync(It.IsAny<UpsertUserForOtpRequest>()), Times.Never);
    _emailService.Verify(s => s.SendVerificationEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
  }

  [Fact]
  public async Task SendOtpForLoginAsync_DatabaseFailure_ReturnsFailureError()
  {
    var email = "test@example.com";
    var otp = "123456";

    _authHelper.Setup(h => h.IsValidEmail(email)).Returns(true);
    _authHelper.Setup(h => h.GenerateOtp()).Returns(otp);
    _authHelper.Setup(h => h.GetPasswordHash(otp, It.IsAny<byte[]>())).Returns(new byte[32]);
    _authRepository.Setup(r => r.UpsertUserForOtpAsync(It.IsAny<UpsertUserForOtpRequest>()))
      .ReturnsAsync(false);

    var result = await _authService.SendOtpForLoginAsync(email);

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().NotBeNull();
    result.Error!.Type.Should().Be(ErrorType.Failure);
    _emailService.Verify(s => s.SendVerificationEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
  }

  [Fact]
  public async Task SendOtpForLoginAsync_EmailServiceFailure_ReturnsEmailServiceError()
  {
    var email = "test@example.com";
    var otp = "123456";
    var emailError = Error.Failure("Email.SendFailed", "Failed to send email");

    _authHelper.Setup(h => h.IsValidEmail(email)).Returns(true);
    _authHelper.Setup(h => h.GenerateOtp()).Returns(otp);
    _authHelper.Setup(h => h.GetPasswordHash(otp, It.IsAny<byte[]>())).Returns(new byte[32]);
    _authRepository.Setup(r => r.UpsertUserForOtpAsync(It.IsAny<UpsertUserForOtpRequest>()))
      .ReturnsAsync(true);
    _emailService.Setup(s => s.SendVerificationEmailAsync(email, otp))
      .ReturnsAsync(ServiceResponse<string>.Failure(emailError));

    var result = await _authService.SendOtpForLoginAsync(email);

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().NotBeNull();
    result.Error!.Code.Should().Be("Email.SendFailed");
    result.Error!.Type.Should().Be(ErrorType.Failure);
  }

  #endregion
}
