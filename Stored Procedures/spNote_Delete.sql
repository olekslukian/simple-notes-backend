USE NotesAppDb

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spNote_Delete
    @NoteID INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM NotesAppSchema.Notes
    WHERE NoteID = @NoteID AND UserId = @UserId;

    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO