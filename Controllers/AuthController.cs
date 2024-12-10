using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleNotesApp.Data;
using SimpleNotesApp.DTO;
using SimpleNotesApp.Helpers;

namespace SimpleNotesApp.Controllers;

[Authorize]
[ApiController]
[Route("controller")]
public class AuthController(IConfiguration config) : ControllerBase, IAuthController
{
    private readonly DbContext db = new(config);
    private readonly AuthHelper _authHelper = new(config);
    public IActionResult Register(UserForRegistrationDTO user)
    {
        throw new NotImplementedException();
    }
    public IActionResult LogIn(UserForLoginDTO user)
    {
        throw new NotImplementedException();
    }


}