USE NotesAppdb

GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spNote_Create
    @UserId INT,
    @Title NVARCHAR(255),
    @Body NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO NotesAppSchema.Notes (
        UserId,
        Title,
        Body,
        CreatedAt,
        UpdatedAt
    )
    OUTPUT INSERTED.NoteId, INSERTED.UserId, INSERTED.Title, INSERTED.Body,
           INSERTED.CreatedAt, INSERTED.UpdatedAt
    VALUES (
        @UserId,
        @Title,
        @Body,
        GETUTCDATE(),
        GETUTCDATE()
    )

END
GO
