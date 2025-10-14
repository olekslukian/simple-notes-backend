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
  public async Task<IActionResult> LogIn(UserForLoginDto user)
  {
    var result = await _authService.LoginAsync(user);

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

  // [HttpPost("register")]
  // public async Task<IActionResult> SetPassword(PasswordSettingDto passwordSettingDto)
  // {
  //   var userId = GetCurrentUserId();

  //   var result = await _authService.SetUserPasswordAsync(passwordSettingDto);

  //   return result.When(
  //       onSuccess: _ => Ok("User registered successfully"),
  //       onFailure: Problem
  //   );
  // }

  [AllowAnonymous]
  [HttpPost("send-email-verification-code")]
  public async Task<IActionResult> SendEmailVerificationCode([FromBody] string email)
  {
    var result = await _authService.SendOtpForEmailVerificationAsync(email);

    return result.When(
        onSuccess: _ => Ok("Verification code sent successfully"),
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
