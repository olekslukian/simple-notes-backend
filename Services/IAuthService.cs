using SimpleNotesApp.Dto.Auth;

namespace SimpleNotesApp.Services;

public interface IAuthService
{
    Task<ServiceResponse<bool>> RegisterUserAsync(UserForRegistrationDto user);
    Task<ServiceResponse<TokensResponseDto>> LoginAsync(UserForLoginDto user);
    Task<ServiceResponse<TokensResponseDto>> RefreshTokenAsync(string refreshToken);
}
