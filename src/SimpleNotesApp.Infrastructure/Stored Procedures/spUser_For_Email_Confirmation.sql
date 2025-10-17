USE NotesAppDb;

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spUser_For_Email_Confirmation
    @Email NVARCHAR(50)
AS

BEGIN

    SELECT
        [UserId],
        [Email],
        [OtpHash],
        [OtpSalt],
        [OtpExpiresAt]
    FROM NotesAppSchema.Auth
        WHERE [Email]=@Email
END
GO
