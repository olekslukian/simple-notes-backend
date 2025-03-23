namespace SimpleNotesApp.DTO;

public partial class TokensResponseDTO(string? accessToken, string? refreshToken)
{
    public string AccessToken { get; set; } = accessToken ?? string.Empty;
    public string RefreshToken { get; set; } = refreshToken ?? string.Empty;
}