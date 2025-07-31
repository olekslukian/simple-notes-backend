using SimpleNotesApp.Dto.Notes;

namespace SimpleNotesApp.Services;

public interface INotesService
{
  Task<ServiceResponse<NoteToGetDto>> CreateNoteAsync(string? userId, NoteToCreateDto note);
  Task<ServiceResponse<NoteToGetDto>> UpdateNoteAsync(string? userId, int noteId, NoteToUpdateDto note);
  Task<ServiceResponse<bool>> DeleteNoteAsync(string? userId, int noteId);
  Task<ServiceResponse<NoteToGetDto>> GetNoteByIdAsync(string? userId, int noteId);
  Task<ServiceResponse<IEnumerable<NoteToGetDto>>> GetNotesAsync(string? userId);
}
