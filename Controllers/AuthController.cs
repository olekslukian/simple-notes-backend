using System.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using SimpleNotesApp.Constants;
using SimpleNotesApp.Data;
using SimpleNotesApp.DTO;
using SimpleNotesApp.Helpers;

namespace SimpleNotesApp.Controllers;

[Authorize]
[ApiController]
[Route("controller")]
public class AuthController(IConfiguration config) : ControllerBase, IAuthController
{
    private readonly DbContext _db = new(config);
    private readonly AuthHelper _authHelper = new(config);

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDTO user)
    {
        if (user.Password != user.PasswordConfirmation)
        {
            return BadRequest("Passwords do not match");
        }

        IEnumerable<string> existingUsers = _db.LoadData<string>(SPConstants.CHECK_USER, new { Email = user.Email });

        if (!existingUsers.IsNullOrEmpty())
        {
            return Conflict("User already exists.");
        }

        byte[] passwordSalt = new byte[16];

        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetNonZeroBytes(passwordSalt);
        }

        byte[] passwordHash = _authHelper.GetPasswordHash(user.Password, passwordSalt);

        var registrationParams = new
        {
            Email = user.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        bool isRegistered = _db.ExecuteSql(SPConstants.REGISTRATION_UPSERT, registrationParams);

        if (isRegistered)
        {
            return Ok(new { Message = "User registered successfully." });
        }

        return StatusCode(500, "Failed to register user.");

    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult LogIn(UserForLoginDTO user)
    {
        // TODO(olekslukian): Fix hash check

        var emailParam = new { Email = user.Email };

        UserForLoginConfirmationDTO? userForConfirmation = _db.LoadDataSingle<UserForLoginConfirmationDTO>(SPConstants.CHECK_USER, emailParam);

        if (userForConfirmation == null)
        {
            return Unauthorized("User does not exist.");
        }

        if (userForConfirmation.PasswordSalt == null || userForConfirmation.PasswordHash == null)
        {
            return StatusCode(500, "User credentials are incomplete.");
        }

        byte[] passwordHash = _authHelper.GetPasswordHash(user.Password, userForConfirmation.PasswordSalt);

        if (passwordHash.Length != userForConfirmation.PasswordHash.Length)
        {
            return StatusCode(500, "Stored password hash is corrupted.");
        }

        bool isPasswordValid = true;
        for (int i = 0; i < passwordHash.Length; i++)
        {
            if (passwordHash[i] != userForConfirmation.PasswordHash[i])
            {
                isPasswordValid = false;
            }
        }

        if (!isPasswordValid)
        {
            return Unauthorized("Invalid password.");
        }

        int userId = _db.LoadDataSingle<int>(SPConstants.USERID_GET, emailParam);

        return Ok(new Dictionary<string, string> { { "token", _authHelper.CreateToken(userId) } });
    }

    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
        throw new NotImplementedException();
    }
}