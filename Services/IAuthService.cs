using SimpleNotesApp.Dto.Auth;

namespace SimpleNotesApp.Services;

public interface IAuthService
{
    ServiceResponse<bool> Register(UserForRegistrationDto user);
    ServiceResponse<TokensResponseDto> Login(UserForLoginDto user);
    ServiceResponse<TokensResponseDto> RefreshToken(string refreshToken);
    ServiceResponse<string> TestAuth(string? userId);
}
