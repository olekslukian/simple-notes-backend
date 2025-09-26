
using SimpleNotesApp.Core.Models;
using SimpleNotesApp.Core.Repositories.Requests;

namespace SimpleNotesApp.Core.Repositories;

public interface INotesRepository
{
  Task<Note?> CreateNoteAsync(CreateNoteRequest request);
  Task<Note?> GetNoteByIdAsync(GetNoteRequest request);
  Task<IEnumerable<Note>> GetNotesByUserIdAsync(int userId);
  Task<Note?> UpdateNoteAsync(UpdateNoteRequest request);
  Task<bool> DeleteNoteAsync(DeleteNoteRequest request);
}
