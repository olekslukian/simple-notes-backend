USE NotesAppDb;

GO


CREATE OR ALTER PROCEDURE NotesAppSchema.spUser_Register
    @Email NVARCHAR(50),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX)
AS

BEGIN
    INSERT INTO NotesAppSchema.Auth (
        [Email],
        [PasswordHash],
        [PasswordSalt]
    ) VALUES (
        @Email,
        @PasswordHash,
        @PasswordSalt
    )
END
