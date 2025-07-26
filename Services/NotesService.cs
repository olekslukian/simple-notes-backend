using Microsoft.IdentityModel.Tokens;
using SimpleNotesApp.Dto.Notes;
using SimpleNotesApp.Models;
using SimpleNotesApp.Repositories;
using SimpleNotesApp.Repositories.Requests;

namespace SimpleNotesApp.Services;

public class NotesService(INotesRepository repository) : INotesService
{
  private readonly INotesRepository _repository = repository;
  public async Task<ServiceResponse<NoteToGetDto>> CreateNoteAsync(string? userId, NoteToCreateDto note)
  {
    if (userId.IsNullOrEmpty())
    {
      return ServiceResponse<NoteToGetDto>.Failure("User not authorized");
    }

    if (note.Title.IsNullOrEmpty() && note.Body.IsNullOrEmpty())
    {
      return ServiceResponse<NoteToGetDto>.Failure("Note should have at least a title or body");
    }

    var request = new CreateNoteRequest(userId!, note.Title, note.Body);

    Note? createdNote = await _repository.CreateNoteAsync(request);

    if (createdNote != null)
    {
      var noteToGetDto = new NoteToGetDto
      {
        NoteId = createdNote.NoteId,
        Title = createdNote.Title,
        Body = createdNote.Body,
        CreatedAt = createdNote.CreatedAt,
        UpdatedAt = createdNote.UpdatedAt
      };

      return ServiceResponse<NoteToGetDto>.Success(noteToGetDto);
    }

    return ServiceResponse<NoteToGetDto>.Failure("Failed to create note");
  }

  public Task<ServiceResponse<bool>> DeleteNoteAsync(int noteId)
  {
    throw new NotImplementedException();
  }

  public Task<ServiceResponse<NoteToGetDto>> GetNoteByIdAsync(int noteId)
  {
    throw new NotImplementedException();
  }

  public Task<ServiceResponse<IEnumerable<NoteToGetDto>>> GetNotesAsync(string? userId)
  {
    throw new NotImplementedException();
  }

  public Task<ServiceResponse<bool>> UpdateNoteAsync(NoteToUpdateDto note)
  {
    throw new NotImplementedException();
  }
}
