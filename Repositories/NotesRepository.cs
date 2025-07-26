using System.Data;
using SimpleNotesApp.Data;
using SimpleNotesApp.Models;
using SimpleNotesApp.Repositories.Requests;

namespace SimpleNotesApp.Repositories;

public class NotesRepository(DbContext db) : INotesRepository
{
  private readonly DbContext _db = db;
  public async Task<Note?> CreateNoteAsync(CreateNoteRequest request)
  {
    var noteParams = new
    {
      UserId = request.UserId,
      Title = request.Title,
      Body = request.Body
    };

    Note? createdNote = await _db.QuerySingleAsync<Note>(SPConstants.CREATE_NOTE, noteParams);

    return createdNote;
  }
  public Task<bool> DeleteNoteAsync(DeleteNoteRequest request)
  {
    throw new NotImplementedException();
  }

  public Task<Note?> GetNoteByIdAsync(GetNoteRequest request)
  {
    throw new NotImplementedException();
  }

  public Task<IEnumerable<Note>> GetNotesByUserIdAsync(GetNotesByUserRequest request)
  {
    throw new NotImplementedException();
  }

  public Task<Note?> UpdateNoteAsync(UpdateNoteRequest request)
  {
    throw new NotImplementedException();
  }
}
