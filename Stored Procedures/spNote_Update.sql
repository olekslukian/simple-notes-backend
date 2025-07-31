USE NotesAppDb

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spNote_Update
    @UserId INT,
    @NoteId INT,
    @Title NVARCHAR(255) = NULL,
    @Body NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE NotesAppSchema.Notes
    SET
        Title = CASE
            WHEN @Title IS NOT NULL AND @Title <> '' THEN @Title
            WHEN @Title IS NULL THEN Title
            ELSE Title
        END,
        Body = CASE
            WHEN @Body IS NOT NULL AND @Body <> '' THEN @Body
            WHEN @Body IS NULL THEN Body
            ELSE Body
        END,
        UpdatedAt = GETUTCDATE()
    OUTPUT INSERTED.NoteId, INSERTED.UserId, INSERTED.Title, INSERTED.Body,
           INSERTED.CreatedAt, INSERTED.UpdatedAt
    WHERE
        NoteId = @NoteId AND UserId = @UserId;
END;
