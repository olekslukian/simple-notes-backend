USE NotesAppDb

GO



CREATE OR ALTEr PROCEDURE NotesAppSchema.spUser_Upsert_for_Otp
    @Email NVARCHAR(255),
    @OtpHash VARBINARY(MAX),
    @OtpSalt VARBINARY(MAX),
    @OtpExpiresAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    MERGE NotesAppSchema.Auth AS T
    USING (SELECT @Email AS Email) AS S
    ON (T.Email = S.Email)

    WHEN MATCHED THEN
        UPDATE SET
            T.OtpHash = @OtpHash,
            T.OtpSalt = @OtpSalt,
            T.OtpExpiresAt = @OtpExpiresAt,
            T.IsEmailVerified = 0

    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Email, OtpHash, OtpSalt, OtpExpiresAt, IsEmailVerified, PasswordHash, PasswordSalt)
        VALUES (
            @Email,
            @OtpHash,
            @OtpSalt,
            @OtpExpiresAt,
            0,
            NULL,
            NULL
        );

    SELECT T.UserId
    FROM NotesAppSchema AS T
    WHERE T.Email = @Email;

END
GO