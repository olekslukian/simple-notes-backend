using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace SimpleNotesApp.Services.Helpers;


public class AuthHelper(IConfiguration config) : IAuthHelper
{
    private readonly IConfiguration _config = config;

    public byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        string passwordKey = _config.GetSection("AppSettings:PasswordKey").Value ?? "";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(passwordKey));

        byte[] combinedSalt = hmac.ComputeHash(passwordSalt);

        return KeyDerivation.Pbkdf2(
            password: password,
            salt: combinedSalt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8
        );
    }

    public string CreateToken(int userId)
    {
        Claim[] claims = [
            new("userId", userId.ToString())
        ];

        string tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value ?? "";
        SymmetricSecurityKey tokenKey = new(Encoding.UTF8.GetBytes(tokenKeyString));
        SigningCredentials credentials = new(tokenKey, SecurityAlgorithms.HmacSha512Signature);
        SecurityTokenDescriptor descriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = credentials
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken token = tokenHandler.CreateToken(descriptor);

        return tokenHandler.WriteToken(token);
    }

    public string CreateRefreshToken()
    {

        var randomNumber = new byte[64];

        using (var numberGenerator = RandomNumberGenerator.Create())
        {
            numberGenerator.GetBytes(randomNumber);
        }

        string base64Url = Convert.ToBase64String(randomNumber)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", string.Empty);

        return base64Url;
    }

}
