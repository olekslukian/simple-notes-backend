using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using SimpleNotesApp.Constants;
using SimpleNotesApp.Context;
using SimpleNotesApp.DTO;
using SimpleNotesApp.Models;
using SimpleNotesApp.Services.Helpers;

namespace SimpleNotesApp.Services;

public class AuthService(IConfiguration config) : IAuthService
{
    private readonly DbContext _db = new(config);
    private readonly AuthHelper _authHelper = new(config);

    public ServiceResponse<bool> Register(UserForRegistrationDTO user)
    {
        if (user.Password != user.PasswordConfirmation)
        {
            return ServiceResponse<bool>.Failure("Passwords do not match");
        }

        IEnumerable<string> existingUsers = _db.LoadData<string>(SPConstants.CHECK_USER, new { Email = user.Email });

        if (!existingUsers.IsNullOrEmpty())
        {
            return ServiceResponse<bool>.Failure("User already exists");
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
            return ServiceResponse<bool>.Success(true);
        }

        return ServiceResponse<bool>.Failure("Failed to register user");

    }
    public ServiceResponse<TokensResponseDTO> Login(UserForLoginDTO user)
    {
        var emailParam = new { Email = user.Email };

        UserForLoginConfirmationDTO? userForConfirmation = _db.LoadDataSingle<UserForLoginConfirmationDTO>(SPConstants.USER_AUTH_CONFIRMATION, emailParam);

        if (userForConfirmation == null)
        {
            return ServiceResponse<TokensResponseDTO>.Failure("Email or password is incorrect");
        }

        if (userForConfirmation.PasswordSalt == null || userForConfirmation.PasswordHash == null)
        {
            return ServiceResponse<TokensResponseDTO>.Failure("Email or password is incorrect");
        }

        byte[] passwordHash = _authHelper.GetPasswordHash(user.Password, userForConfirmation.PasswordSalt);

        if (passwordHash.Length != userForConfirmation.PasswordHash.Length)
        {
            return ServiceResponse<TokensResponseDTO>.Failure("Email or password is incorrect");
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
            return ServiceResponse<TokensResponseDTO>.Failure("Email or password is incorrect");
        }

        int userId = _db.LoadDataSingle<int>(SPConstants.USERID_GET, emailParam);

        string refreshToken = CreateAndSaveRefreshToken(userId);

        if (refreshToken.IsNullOrEmpty())
        {
            return ServiceResponse<TokensResponseDTO>.Failure("Something went wrong");
        }

        return ServiceResponse<TokensResponseDTO>.Success(new(accessToken: _authHelper.CreateToken(userId), refreshToken: refreshToken));
    }
    public ServiceResponse<TokensResponseDTO> RefreshToken(string refreshToken)
    {
        var tokenParam = new { RefreshToken = refreshToken };

        User? userFromDb = _db.LoadDataSingle<User>(SPConstants.GET_USER_BY_REF_TOKEN, tokenParam);

        if (userFromDb == null)
        {
            return ServiceResponse<TokensResponseDTO>.Failure("Failed to refresh token");
        }

        if (refreshToken != userFromDb.RefreshToken)
        {
            return ServiceResponse<TokensResponseDTO>.Failure("Refresh token is invalid");
        }

        if (userFromDb.RefreshTokenExpires.CompareTo(DateTime.UtcNow) < 0)
        {
            return ServiceResponse<TokensResponseDTO>.Failure("Refresh token is expired");
        }

        string newAccessToken = _authHelper.CreateToken(userFromDb.UserId);
        string newRefreshToken = CreateAndSaveRefreshToken(userFromDb.UserId);

        return ServiceResponse<TokensResponseDTO>.Success(new(accessToken: newAccessToken, refreshToken: newRefreshToken));
    }

    private string CreateAndSaveRefreshToken(int userId)
    {
        var expirationDate = DateTime.UtcNow.AddDays(7);

        var token = _authHelper.CreateRefreshToken();

        var tokenParams = new
        {
            UserId = userId,
            RefreshToken = token,
            RefreshTokenExpires = expirationDate
        };

        var result = _db.ExecuteSql(SPConstants.REFRESH_TOKEN_UPDATE, tokenParams);

        if (!result)
        {
            return "";
        }

        return token;
    }


}