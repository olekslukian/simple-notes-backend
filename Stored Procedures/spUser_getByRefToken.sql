USE NotesAppDb

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spUser_getByRefToken
    @RefreshToken NVARCHAR(100)
AS

BEGIN
    SELECT [UserId], 
            [Email], 
            [PasswordHash], 
            [PasswordSalt], 
            [RefreshToken], 
            [RefreshTokenExpires] 
        FROM NotesAppSchema.Auth 
        WHERE RefreshToken = @RefreshToken
END
GO
