USE NotesAppDb

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spUser_getForPasswordChange
    @UserId  INT
AS

BEGIN
    SELECT [UserId],
            [PasswordHash],
            [PasswordSalt]
        FROM NotesAppSchema.Auth
        WHERE UserId = @UserId
END
GO
