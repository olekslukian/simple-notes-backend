USE NotesAppDb

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spUser_getEmailById
    @UserId NVARCHAR(50)
AS

BEGIN
    SELECT [Email]
        FROM NotesAppSchema.Auth 
        WHERE UserId = @UserId
END
GO
