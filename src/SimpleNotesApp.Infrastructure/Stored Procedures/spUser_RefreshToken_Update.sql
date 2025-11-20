USE NotesAppDb;
GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spUser_RefreshToken_Update
    @UserId INT,
    @RefreshToken NVARCHAR(MAX),
    @RefreshTokenExpiresAt DATETIME
AS
BEGIN
    UPDATE NotesAppSchema.Auth
    SET RefreshToken = @RefreshToken,
        RefreshTokenExpiresAt = @RefreshTokenExpiresAt
    WHERE UserId = @UserId;
END
GO
