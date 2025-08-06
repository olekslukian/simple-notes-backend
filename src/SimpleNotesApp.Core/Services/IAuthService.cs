
using SimpleNotesApp.Core.Common;
using SimpleNotesApp.Core.Dto.Auth;

namespace SimpleNotesApp.Core.Services;

public interface IAuthService
{
    Task<ServiceResponse<bool>> RegisterUserAsync(UserForRegistrationDto user);
    Task<ServiceResponse<TokensResponseDto>> LoginAsync(UserForLoginDto user);
    Task<ServiceResponse<TokensResponseDto>> RefreshTokenAsync(string refreshToken);
}
