using Microsoft.AspNetCore.Mvc;
using SimpleNotesApp.DTO;

namespace SimpleNotesApp.Controllers;

public interface IAuthController
{
    public IActionResult Register(UserForRegistrationDTO user);
    public IActionResult LogIn(UserForLoginDTO user);
}