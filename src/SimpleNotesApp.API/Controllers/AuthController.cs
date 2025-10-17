using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleNotesApp.Core.Dto.Auth;
using SimpleNotesApp.Core.Services;
namespace SimpleNotesApp.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : BaseController
{
  private readonly IAuthService _authService = authService;

  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<IActionResult> LogInWithPassword(UserForLoginDto user)
  {
    var result = await _authService.LoginWithPasswordAsync(user);

    return result.When(
        onSuccess: Ok,
        onFailure: Problem
    );
  }

  [AllowAnonymous]
  [HttpGet("refresh-token")]
  public async Task<IActionResult> RefreshToken(string refreshToken)
  {

    var result = await _authService.RefreshTokenAsync(refreshToken);

    return result.When(
        onSuccess: Ok,
        onFailure: Problem
    );
  }

  [AllowAnonymous]
  [HttpPost("send-otp-for-login")]
  public async Task<IActionResult> SendOtpForLogin([FromBody] string email)
  {
    var result = await _authService.SendOtpForLoginAsync(email);

    return result.When(
        onSuccess: _ => Ok("Verification code sent successfully"),
        onFailure: Problem
    );
  }

  [AllowAnonymous]
  [HttpPost("verify-email-for-login")]
  public async Task<IActionResult> VerifyEmailForLogin(VerifyEmailForLoginDto verifyEmailForLoginDto)
  {
    var result = await _authService.VerifyEmailForLoginAsync(verifyEmailForLoginDto);

    return result.When(
        onSuccess: Ok,
        onFailure: Problem
    );
  }

  [HttpPost("send-otp-for-password-set")]
  public async Task<IActionResult> SendOtpForPasswordSet()
  {
    var userId = GetCurrentUserId();

    var result = await _authService.SendOtpForPasswordSetAsync(userId);

    return result.When(
        onSuccess: email => Ok(email),
        onFailure: Problem
    );
  }

  [HttpPost("set-password")]
  public async Task<IActionResult> SetPassword(PasswordSettingDto passwordSettingDto)
  {
    var userId = GetCurrentUserId();

    var result = await _authService.SetUserPasswordAsync(userId, passwordSettingDto);

    return result.When(
        onSuccess: _ => Ok("Password set successfully"),
        onFailure: Problem
    );
  }

  [HttpPatch("change-password")]
  public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
  {
    var userId = GetCurrentUserId();

    var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);

    return result.When(
        onSuccess: _ => Ok("Password changed successfully"),
        onFailure: Problem
    );
  }
}
