namespace SimpleNotesApp.Infrastructure.Repositories.Requests;

public record RegisterUserRequest(
  string Email,
  byte[] PasswordHash,
  byte[] PasswordSalt
);

public record UpdateRefreshTokenRequest(
  int UserId,
  string RefreshToken,
  DateTime RefreshTokenExpires
);

public record ChangePasswordRequest(
  int UserId,
  byte[] PasswordHash,
  byte[] PasswordSalt
);
