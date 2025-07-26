using SimpleNotesApp.Dto.Notes;

namespace SimpleNotesApp.Services;

public interface INotesService
{
  Task<ServiceResponse<NoteToGetDto>> CreateNoteAsync(string? userId, NoteToCreateDto note);
  Task<ServiceResponse<bool>> UpdateNoteAsync(NoteToUpdateDto note);
  Task<ServiceResponse<bool>> DeleteNoteAsync(int noteId);
  Task<ServiceResponse<NoteToGetDto>> GetNoteByIdAsync(int noteId);
  Task<ServiceResponse<IEnumerable<NoteToGetDto>>> GetNotesAsync(string? userId);
}
