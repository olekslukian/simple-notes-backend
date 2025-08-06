USE NotesAppDb;
GO

CREATE OR ALTER PROCEDURE NotesAppSchema.spRegistration_Upsert
    @Email NVARCHAR(50),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX)
AS
    BEGIN
        IF NOT EXISTS(SELECT * FROM NotesAppSchema.Auth WHERE Email = @Email)
            BEGIN
                INSERT INTO NotesAppSchema.Auth(
                    Email,
                    PasswordHash,
                    PasswordSalt
                ) VALUES (
                    @Email,
                    @PasswordHash,
                    @PasswordSalt
                )
            END
        ELSE 
            BEGIN
                UPDATE NotesAppSchema.Auth
                    SET PasswordHash = @PasswordHash,
                        PasswordSalt = @PasswordSalt
                    WHERE Email = @Email
            END
    END
GO