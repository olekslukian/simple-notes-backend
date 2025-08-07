USE NotesAppDb

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spNote_Delete
    @NoteId INT,
    @UserId INT
AS
BEGIN
    DELETE FROM NotesAppSchema.Notes
    WHERE NoteId = @NoteId AND UserId = @UserId;

END;
GO
