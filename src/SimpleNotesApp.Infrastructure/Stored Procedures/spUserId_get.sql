USE NotesAppDb

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spUserId_get
    @Email NVARCHAR(50)
AS

BEGIN
    SELECT [UserId] FROM NotesAppSchema.Auth 
        WHERE Email = @Email
END
GO


