namespace SimpleNotesApp.Core.Repositories.Requests;

public record CreateNoteRequest(
    int UserId,
    string Title,
    string Body
);

public record UpdateNoteRequest(
    int NoteId,
    int UserId,
    string? Title,
    string? Body
);

public record GetNoteRequest(
    int NoteId,
    int UserId
);

public record DeleteNoteRequest(
    int NoteId,
    int UserId
);
