namespace SimpleNotesApp.Dto.Auth;

public partial class TokensResponseDto(string? accessToken, string? refreshToken)
{
    public string AccessToken { get; set; } = accessToken ?? string.Empty;
    public string RefreshToken { get; set; } = refreshToken ?? string.Empty;
}
