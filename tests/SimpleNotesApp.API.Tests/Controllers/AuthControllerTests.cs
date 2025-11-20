using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleNotesApp.API.Controllers;
using SimpleNotesApp.Core.Common;
using SimpleNotesApp.Core.Dto.Auth;
using SimpleNotesApp.Core.Services;
using System.Security.Claims;

namespace SimpleNotesApp.API.Tests.Controllers;

public class AuthControllerTests
{
  private readonly Mock<IAuthService> _authService;
  private readonly AuthController _controller;

  public AuthControllerTests()
  {
    _authService = new Mock<IAuthService>();
    _controller = new AuthController(_authService.Object);
  }

  #region LogInWithPassword Tests

  [Fact]
  public async Task LogInWithPassword_ValidCredentials_ReturnsOkWithTokens()
  {
    var loginDto = new UserForLoginDto
    {
      Email = "test@example.com",
      Password = "password123"
    };

    var tokensResponse = new TokensResponseDto
    {
      AccessToken = "test-access-token",
      RefreshToken = "test-refresh-token"
    };

    _authService.Setup(s => s.LoginWithPasswordAsync(loginDto))
      .ReturnsAsync(ServiceResponse<TokensResponseDto>.Success(tokensResponse));

    var result = await _controller.LogInWithPassword(loginDto);

    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    okResult.StatusCode.Should().Be(200);
    okResult.Value.Should().BeEquivalentTo(tokensResponse);
  }

  [Fact]
  public async Task LogInWithPassword_InvalidCredentials_ReturnsUnauthorized()
  {
    var loginDto = new UserForLoginDto
    {
      Email = "test@example.com",
      Password = "wrongpassword"
    };

    var error = Error.Unauthorized("Auth.InvalidCredentials", "Invalid credentials");
    _authService.Setup(s => s.LoginWithPasswordAsync(loginDto))
      .ReturnsAsync(ServiceResponse<TokensResponseDto>.Failure(error));

    var result = await _controller.LogInWithPassword(loginDto);

    var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
    objectResult.StatusCode.Should().Be(401);
  }

  #endregion

  #region RefreshToken Tests

  [Fact]
  public async Task RefreshToken_ValidToken_ReturnsOkWithNewTokens()
  {
    var refreshToken = "valid-refresh-token";
    var tokensResponse = new TokensResponseDto
    {
      AccessToken = "new-access-token",
      RefreshToken = "new-refresh-token"
    };

    _authService.Setup(s => s.RefreshTokenAsync(refreshToken))
      .ReturnsAsync(ServiceResponse<TokensResponseDto>.Success(tokensResponse));

    var result = await _controller.RefreshToken(refreshToken);

    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    okResult.Value.Should().BeEquivalentTo(tokensResponse);
  }

  [Fact]
  public async Task RefreshToken_ExpiredToken_ReturnsUnauthorized()
  {
    var refreshToken = "expired-token";
    var error = Error.Unauthorized("Auth.TokenExpired", "Refresh token expired");

    _authService.Setup(s => s.RefreshTokenAsync(refreshToken))
      .ReturnsAsync(ServiceResponse<TokensResponseDto>.Failure(error));

    var result = await _controller.RefreshToken(refreshToken);

    var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
    objectResult.StatusCode.Should().Be(401);
  }

  #endregion

  #region SendOtpForLogin Tests

  [Fact]
  public async Task SendOtpForLogin_ValidEmail_ReturnsOk()
  {
    var email = "test@example.com";
    var obscuredEmail = "t**t@example.com";

    _authService.Setup(s => s.SendOtpForLoginAsync(email))
      .ReturnsAsync(ServiceResponse<string>.Success(obscuredEmail));

    var result = await _controller.SendOtpForLogin(email);

    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    okResult.StatusCode.Should().Be(200);
    okResult.Value.Should().Be("Verification code sent successfully");
  }

  [Fact]
  public async Task SendOtpForLogin_InvalidEmail_ReturnsBadRequest()
  {
    var email = "invalid-email";
    var error = Error.Validation("Auth.InvalidEmail", "Invalid email format");

    _authService.Setup(s => s.SendOtpForLoginAsync(email))
      .ReturnsAsync(ServiceResponse<string>.Failure(error));

    var result = await _controller.SendOtpForLogin(email);

    var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
    var problemDetails = objectResult.Value.Should().BeAssignableTo<ProblemDetails>().Subject;
    problemDetails.Status.Should().Be(400);
  }

  #endregion

  #region VerifyEmailForLogin Tests

  [Fact]
  public async Task VerifyEmailForLogin_ValidOtp_ReturnsOkWithTokens()
  {
    var verifyDto = new VerifyEmailForLoginDto
    {
      Email = "test@example.com",
      Otp = "123456"
    };

    var tokensResponse = new TokensResponseDto
    {
      AccessToken = "access-token",
      RefreshToken = "refresh-token"
    };

    _authService.Setup(s => s.VerifyEmailForLoginAsync(verifyDto))
      .ReturnsAsync(ServiceResponse<TokensResponseDto>.Success(tokensResponse));

    var result = await _controller.VerifyEmailForLogin(verifyDto);

    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    okResult.Value.Should().BeEquivalentTo(tokensResponse);
  }

  [Fact]
  public async Task VerifyEmailForLogin_InvalidOtp_ReturnsUnauthorized()
  {
    var verifyDto = new VerifyEmailForLoginDto
    {
      Email = "test@example.com",
      Otp = "wrong-otp"
    };

    var error = Error.Unauthorized("Auth.InvalidOtp", "Invalid or expired OTP");

    _authService.Setup(s => s.VerifyEmailForLoginAsync(verifyDto))
      .ReturnsAsync(ServiceResponse<TokensResponseDto>.Failure(error));

    var result = await _controller.VerifyEmailForLogin(verifyDto);

    var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
    objectResult.StatusCode.Should().Be(401);
  }

  #endregion

  #region SendOtpForPasswordSet Tests

  [Fact]
  public async Task SendOtpForPasswordSet_AuthenticatedUser_ReturnsOk()
  {
    var userId = 1;
    var obscuredEmail = "t**t@example.com";

    var claims = new List<Claim> { new Claim("userId", userId.ToString()) };
    var identity = new ClaimsIdentity(claims, "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = claimsPrincipal }
    };

    _authService.Setup(s => s.SendOtpForPasswordSetAsync(userId))
      .ReturnsAsync(ServiceResponse<string>.Success(obscuredEmail));

    var result = await _controller.SendOtpForPasswordSet();

    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    okResult.Value.Should().Be(obscuredEmail);
  }

  #endregion

  #region SetPassword Tests

  [Fact]
  public async Task SetPassword_ValidPassword_ReturnsOk()
  {
    var userId = 1;
    var passwordDto = new PasswordSettingDto
    {
      Otp = "123456",
      Password = "NewPassword123!"
    };

    var claims = new List<Claim> { new Claim("userId", userId.ToString()) };
    var identity = new ClaimsIdentity(claims, "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = claimsPrincipal }
    };

    _authService.Setup(s => s.SetUserPasswordAsync(userId, passwordDto))
      .ReturnsAsync(ServiceResponse<bool>.Success(true));

    var result = await _controller.SetPassword(passwordDto);

    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    okResult.Value.Should().Be("Password set successfully");
  }

  [Fact]
  public async Task SetPassword_InvalidOtp_ReturnsBadRequest()
  {
    var userId = 1;
    var passwordDto = new PasswordSettingDto
    {
      Otp = "wrong-otp",
      Password = "NewPassword123!"
    };

    var claims = new List<Claim> { new Claim("userId", userId.ToString()) };
    var identity = new ClaimsIdentity(claims, "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = claimsPrincipal }
    };

    var error = Error.Validation("Auth.InvalidOtp", "Invalid OTP");

    _authService.Setup(s => s.SetUserPasswordAsync(userId, passwordDto))
      .ReturnsAsync(ServiceResponse<bool>.Failure(error));

    var result = await _controller.SetPassword(passwordDto);

    var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
    var problemDetails = objectResult.Value.Should().BeAssignableTo<ProblemDetails>().Subject;
    problemDetails.Status.Should().Be(400);
  }

  #endregion

  #region ChangePassword Tests

  [Fact]
  public async Task ChangePassword_ValidPasswords_ReturnsOk()
  {
    var userId = 1;
    var changePasswordDto = new ChangePasswordDto
    {
      OldPassword = "OldPassword123!",
      NewPassword = "NewPassword123!"
    };

    var claims = new List<Claim> { new Claim("userId", userId.ToString()) };
    var identity = new ClaimsIdentity(claims, "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = claimsPrincipal }
    };

    _authService.Setup(s => s.ChangePasswordAsync(userId, changePasswordDto))
      .ReturnsAsync(ServiceResponse<bool>.Success(true));

    var result = await _controller.ChangePassword(changePasswordDto);

    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    okResult.Value.Should().Be("Password changed successfully");
  }

  [Fact]
  public async Task ChangePassword_IncorrectOldPassword_ReturnsUnauthorized()
  {
    var userId = 1;
    var changePasswordDto = new ChangePasswordDto
    {
      OldPassword = "WrongPassword",
      NewPassword = "NewPassword123!"
    };

    var claims = new List<Claim> { new Claim("userId", userId.ToString()) };
    var identity = new ClaimsIdentity(claims, "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = claimsPrincipal }
    };

    var error = Error.Unauthorized("Auth.InvalidPassword", "Incorrect old password");

    _authService.Setup(s => s.ChangePasswordAsync(userId, changePasswordDto))
      .ReturnsAsync(ServiceResponse<bool>.Failure(error));

    var result = await _controller.ChangePassword(changePasswordDto);

    var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
    objectResult.StatusCode.Should().Be(401);
  }

  #endregion
}
