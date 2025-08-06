USE NotesAppDb;
GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spUser_Delete
    @UserId INT
AS

BEGIN
    DELETE FROM NotesAppSchema.Auth WHERE UserId = @UserId
END
GO