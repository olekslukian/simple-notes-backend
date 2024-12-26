USE NotesAppDb;

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spUser_Auth_Confirmation
    @Email NVARCHAR(50)
AS

BEGIN

    SELECT 
        [Email],
        [PasswordHash],
        [PasswordSalt] 
    FROM NotesAppSchema.Auth
        WHERE [Email]=@Email

END
