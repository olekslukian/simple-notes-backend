
using SimpleNotesApp.Infrastructure.Models;
using SimpleNotesApp.Infrastructure.Repositories.Requests;

namespace SimpleNotesApp.Infrastructure.Repositories;

public interface INotesRepository
{
  Task<Note?> CreateNoteAsync(CreateNoteRequest request);
  Task<Note?> GetNoteByIdAsync(GetNoteRequest request);
  Task<IEnumerable<Note>> GetNotesByUserIdAsync(int userId);
  Task<Note?> UpdateNoteAsync(UpdateNoteRequest request);
  Task<bool> DeleteNoteAsync(DeleteNoteRequest request);
}
