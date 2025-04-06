using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SimpleNotesApp.DTO;
using SimpleNotesApp.Services;
namespace SimpleNotesApp.Controllers;

[Authorize]
[ApiController]
[Route("Auth")]
public class AuthController(IAuthService authService) : ControllerBase, IAuthController
{
    private readonly IAuthService _authService = authService;

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDTO user)
    {
        var result = _authService.Register(user);

        return result.When(
            onSuccess: _ => Ok("User registered successfully"),
            onFailure: BadRequest
        );
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult LogIn(UserForLoginDTO user)
    {
        var result = _authService.Login(user);

        return result.When(
            onSuccess: Ok,
            onFailure: Unauthorized
        );
    }

    [AllowAnonymous]
    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken(string refreshToken)
    {

        var result = _authService.RefreshToken(refreshToken);

        return result.When(
            onSuccess: Ok,
            onFailure: Unauthorized
        );
    }

    [HttpGet("TestAuth")]
    public IActionResult TestAuth()
    {
        return Ok("Worked successfully");
    }
}