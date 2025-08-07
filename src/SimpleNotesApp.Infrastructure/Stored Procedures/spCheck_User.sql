USE NotesAppDb

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spCheck_User
    @Email NVARCHAR(50)
AS

BEGIN
    SELECT [Email] FROM NotesAppSchema.Auth 
        WHERE Email = @Email
END
GO
