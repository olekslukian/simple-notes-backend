USE NotesAppDb

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spNotes_GetAllByUserId
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        NoteId,
        UserId,
        Title,
        Body,
        CreatedAt,
        UpdatedAt
    FROM NotesAppSchema.Notes
    WHERE UserId = @UserId
    ORDER BY UpdatedAt DESC;
END;
GO
