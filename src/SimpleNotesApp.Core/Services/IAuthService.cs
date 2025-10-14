
using SimpleNotesApp.Core.Common;
using SimpleNotesApp.Core.Dto.Auth;

namespace SimpleNotesApp.Core.Services;

public interface IAuthService
{
    Task<ServiceResponse<TokensResponseDto>> LoginAsync(UserForLoginDto user);
    Task<ServiceResponse<TokensResponseDto>> RefreshTokenAsync(string refreshToken);
    Task<ServiceResponse<bool>> SetUserPasswordAsync(int userId, PasswordSettingDto passwordDto);
    Task<ServiceResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    Task<ServiceResponse<bool>> SendOtpForEmailVerificationAsync(string email);
}
