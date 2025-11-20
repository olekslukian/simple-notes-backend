namespace SimpleNotesApp.Core.Dto.Auth;

public partial class TokensResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public TokensResponseDto()
    {
        AccessToken ??= string.Empty;
        RefreshToken ??= string.Empty;
    }
}
