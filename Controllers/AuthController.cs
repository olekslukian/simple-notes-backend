using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleNotesApp.Dto.Auth;
using SimpleNotesApp.Services;
namespace SimpleNotesApp.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
  private readonly IAuthService _authService = authService;

  [AllowAnonymous]
  [HttpPost("register")]
  public async Task<IActionResult> Register(UserForRegistrationDto user)
  {
    var result = await _authService.RegisterUserAsync(user);

    return result.When(
        onSuccess: _ => Ok("User registered successfully"),
        onFailure: BadRequest
    );
  }

  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<IActionResult> LogIn(UserForLoginDto user)
  {
    var result = await _authService.LoginAsync(user);

    return result.When(
        onSuccess: Ok,
        onFailure: Unauthorized
    );
  }

  [AllowAnonymous]
  [HttpGet("refresh-token")]
  public async Task<IActionResult> RefreshToken(string refreshToken)
  {

    var result = await _authService.RefreshTokenAsync(refreshToken);

    return result.When(
        onSuccess: Ok,
        onFailure: Unauthorized
    );
  }
}
