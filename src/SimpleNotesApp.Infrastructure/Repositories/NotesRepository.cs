using SimpleNotesApp.Core.Models;
using SimpleNotesApp.Core.Repositories;
using SimpleNotesApp.Core.Repositories.Requests;
using SimpleNotesApp.Infrastructure.Data;

namespace SimpleNotesApp.Infrastructure.Repositories;

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

    Note? createdNote = await _db.QuerySingleAsync<Note>(SP.CREATE_NOTE, noteParams);

    return createdNote;
  }
  public async Task<bool> DeleteNoteAsync(DeleteNoteRequest request)
  {
    var parameters = new
    {
      NoteId = request.NoteId,
      UserId = request.UserId
    };

    return await _db.ExecuteAsync(SP.DELETE_NOTE, parameters);
  }

  public Task<Note?> GetNoteByIdAsync(GetNoteRequest request)
  {
    var parameters = new
    {
      NoteId = request.NoteId,
      UserId = request.UserId
    };

    return _db.QuerySingleAsync<Note>(SP.GET_NOTE_BY_ID, parameters);
  }

  public Task<IEnumerable<Note>> GetNotesByUserIdAsync(int userId)
  {
    var parameters = new
    {
      UserId = userId
    };

    return _db.QueryAsync<Note>(SP.GET_NOTES_BY_USER_ID, parameters);
  }

  public async Task<Note?> UpdateNoteAsync(UpdateNoteRequest request)
  {
    var parameters = new
    {
      UserId = request.UserId,
      NoteId = request.NoteId,
      Title = request.Title,
      Body = request.Body
    };

    return await _db.QuerySingleAsync<Note>(SP.UPDATE_NOTE, parameters);
  }
}
