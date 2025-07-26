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
  public IActionResult Register(UserForRegistrationDto user)
  {
    var result = _authService.Register(user);

    return result.When(
        onSuccess: _ => Ok("User registered successfully"),
        onFailure: BadRequest
    );
  }

  [AllowAnonymous]
  [HttpPost("login")]
  public IActionResult LogIn(UserForLoginDto user)
  {
    var result = _authService.Login(user);

    return result.When(
        onSuccess: Ok,
        onFailure: Unauthorized
    );
  }

  [AllowAnonymous]
  [HttpGet("refresh-token")]
  public IActionResult RefreshToken(string refreshToken)
  {

    var result = _authService.RefreshToken(refreshToken);

    return result.When(
        onSuccess: Ok,
        onFailure: Unauthorized
    );
  }

  [HttpGet("test-auth")]
  public IActionResult TestAuth()
  {
    var userId = User.FindFirst("userId")?.Value;

    var result = _authService.TestAuth(userId);

    return result.When(
        onSuccess: id => Ok($"Authenticated user ID: {id}"),
        onFailure: Unauthorized
    );
  }
}
