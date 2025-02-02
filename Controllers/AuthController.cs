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

        string sqlCheckUserExists = @$"EXEC {StoredProceduresConstants.CHECK_USER} @Email={user.Email}";

        IEnumerable<string> existingUsers = _db.LoadData<string>(sqlCheckUserExists);

        if (existingUsers.IsNullOrEmpty())
        {
            byte[] passwordSalt = new byte[128 / 8];

            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetNonZeroBytes(passwordSalt);

            byte[] passwordHash = _authHelper.GetPasswordHash(user.Password, passwordSalt);


            //  TODO(olekslukian): Figure out how to use correctly stored procedures with parameters

            string registrationUpsertSql = @"EXEC NotesAppSchema.spRegistration_Upsert @Email=" + user.Email;

            List<SqlParameter> parameters = [];

            SqlParameter passwordHashParameter = new("@PasswordHash", SqlDbType.VarBinary)
            {
                Value = passwordHash
            };

            SqlParameter passwordSaltParameter = new("@PasswordSalt", SqlDbType.VarBinary)
            {
                Value = passwordSalt
            };

            parameters.Add(passwordHashParameter);
            parameters.Add(passwordSaltParameter);


            if (_db.ExecuteSqlWithParameters(registrationUpsertSql, parameters))
            {
                return Ok();
            }

            throw new Exception("Failed to register user");
        }

        throw new Exception("This user already exists");

    }
    [HttpPost("Login")]
    public IActionResult LogIn(UserForLoginDTO user)
    {
        throw new NotImplementedException();
    }

    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
        throw new NotImplementedException();
    }
}