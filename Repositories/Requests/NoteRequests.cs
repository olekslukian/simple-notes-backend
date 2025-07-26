namespace SimpleNotesApp.Repositories.Requests;

public record CreateNoteRequest(
    string UserId,
    string Title,
    string Body
);

public record UpdateNoteRequest(
    int NoteId,
    string UserId,
    string Title,
    string Body
);

public record GetNoteRequest(
    int NoteId
);

public record GetNotesByUserRequest(
    string UserId
);

public record DeleteNoteRequest(
    int NoteId
);
