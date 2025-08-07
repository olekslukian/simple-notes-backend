USE NotesAppDb

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spPassword_Change
    @UserId INT,
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX)
AS
    BEGIN
        UPDATE NotesAppSchema.Auth
        SET PasswordHash = @PasswordHash,
            PasswordSalt = @PasswordSalt
        WHERE UserId = @UserId
    END
GO
