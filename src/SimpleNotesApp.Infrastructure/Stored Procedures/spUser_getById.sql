USE NotesAppDb

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spUser_getById
    @UserId NVARCHAR(50)
AS

BEGIN
    SELECT [UserId], 
            [Email], 
            [PasswordHash], 
            [PasswordSalt], 
            [RefreshToken], 
            [RefreshTokenExpires] 
        FROM NotesAppSchema.Auth 
        WHERE UserId = @UserId
END
GO
