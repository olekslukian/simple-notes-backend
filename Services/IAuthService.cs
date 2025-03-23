using SimpleNotesApp.DTO;

namespace SimpleNotesApp.Services;

public interface IAuthService
{
    ServiceResponse<bool> Register(UserForRegistrationDTO user);
    ServiceResponse<TokensResponseDTO> Login(UserForLoginDTO user);
    ServiceResponse<TokensResponseDTO> RefreshToken(string userId, string refreshToken);
}