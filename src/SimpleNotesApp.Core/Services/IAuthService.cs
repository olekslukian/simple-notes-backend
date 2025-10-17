
using SimpleNotesApp.Core.Common;
using SimpleNotesApp.Core.Dto.Auth;

namespace SimpleNotesApp.Core.Services;

public interface IAuthService
{
    Task<ServiceResponse<TokensResponseDto>> LoginWithPasswordAsync(UserForLoginDto user);
    Task<ServiceResponse<TokensResponseDto>> RefreshTokenAsync(string refreshToken);
    Task<ServiceResponse<bool>> SetUserPasswordAsync(int userId, PasswordSettingDto passwordDto);
    Task<ServiceResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    Task<ServiceResponse<string>> SendOtpForPasswordSetAsync(int userId);
    Task<ServiceResponse<string>> SendOtpForLoginAsync(string email);
    Task<ServiceResponse<TokensResponseDto>> VerifyEmailForLoginAsync(VerifyEmailForLoginDto verifyEmailForLoginDto);
}
