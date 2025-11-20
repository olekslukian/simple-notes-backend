USE NotesAppDb;

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spUser_Auth_Confirmation
    @Email NVARCHAR(50)
AS

BEGIN

    SELECT
        [UserId],
        [Email],
        [PasswordHash],
        [PasswordSalt],
        [IsEmailVerified]
    FROM NotesAppSchema.Auth
        WHERE [Email]=@Email

END
GO
