USE NotesAppDb;
GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spUser_RefreshToken_Update
    @UserId INT,
    @RefreshToken NVARCHAR(MAX),
    @RefreshTokenExpires DATETIME
AS
BEGIN
    UPDATE NotesAppSchema.Auth
    SET RefreshToken = @RefreshToken,
        RefreshTokenExpires = @RefreshTokenExpires
    WHERE UserId = @UserId;
END
GO
