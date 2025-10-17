USE NotesAppDb;
GO


CREATE OR ALTER PROCEDURE NotesAppSchema.spUser_Upsert_Otp
    @Email NVARCHAR(255),
    @OtpHash VARBINARY(MAX),
    @OtpSalt VARBINARY(MAX),
    @OtpExpiresAt DATETIME2
AS
BEGIN
    MERGE NotesAppSchema.Auth AS T
    USING (SELECT @Email AS Email) AS S
    ON (T.Email = S.Email)

    WHEN MATCHED THEN
        UPDATE SET
            T.OtpHash = @OtpHash,
            T.OtpSalt = @OtpSalt,
            T.OtpExpiresAt = @OtpExpiresAt

    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Email, OtpHash, OtpSalt, OtpExpiresAt, IsEmailVerified)
        VALUES (
            @Email,
            @OtpHash,
            @OtpSalt,
            @OtpExpiresAt,
            0            
        );

END
GO