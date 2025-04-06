namespace SimpleNotesApp.Constants
{
    public class SPConstants
    {
        public const string CHECK_USER = "NotesAppSchema.spCheck_User";
        public const string REGISTRATION_UPSERT = "NotesAppSchema.spRegistration_Upsert";
        public const string USER_AUTH_CONFIRMATION = "NotesAppSchema.spUser_Auth_Confirmation";
        public const string USER_DELETE = "NotesAppSchema.spUser_Delete";
        public const string USERID_GET = "NotesAppSchema.spUserId_get";
        public const string REFRESH_TOKEN_UPDATE = "NotesAppSchema.spUser_RefreshToken_Update";
        public const string GET_USER_BY_ID = "NotesAppSchema.spUser_getById";
        public const string GET_USER_BY_REF_TOKEN = "NotesAppSchema.spUser_getByRefToken";
    }
}