USE NotesAppDb

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spUser_Set_Email_Verified
    @UserId INT
AS

BEGIN
    UPDATE NotesAppSchema.Auth
    SET IsEmailVerified = 1
        WHERE UserId = @UserId
END
GO
