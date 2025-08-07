USE NotesAppDb

GO


CREATE OR ALTER PROCEDURE NotesAppSchema.spNote_GetById
    @NoteId INT,
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
    WHERE NoteId = @NoteId AND UserId = @UserId;
END;
GO
