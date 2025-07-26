CREATE OR ALTER PROCEDURE NotesAppSchema.spNote_GetById
    @NoteID INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        NoteID,
        UserId,
        Title,
        Body,
        CreatedAt,
        UpdatedAt
    FROM NotesAppSchema.Notes
    WHERE NoteID = @NoteID AND UserId = @UserId;
END;
GO