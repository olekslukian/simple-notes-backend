
using SimpleNotesApp.Core.Common;
using SimpleNotesApp.Core.Dto.Notes;

namespace SimpleNotesApp.Core.Services;

public interface INotesService
{
  Task<ServiceResponse<NoteToGetDto>> CreateNoteAsync(int userId, NoteToCreateDto note);
  Task<ServiceResponse<NoteToGetDto>> UpdateNoteAsync(int userId, int noteId, NoteToUpdateDto note);
  Task<ServiceResponse<bool>> DeleteNoteAsync(int userId, int noteId);
  Task<ServiceResponse<NoteToGetDto>> GetNoteByIdAsync(int userId, int noteId);
  Task<ServiceResponse<IEnumerable<NoteToGetDto>>> GetNotesAsync(int userId);
}
