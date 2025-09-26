using SimpleNotesApp.Core.Common;
using SimpleNotesApp.Core.Dto.Notes;
using SimpleNotesApp.Core.Models;
using SimpleNotesApp.Core.Repositories;
using SimpleNotesApp.Core.Repositories.Requests;

namespace SimpleNotesApp.Core.Services;

public class NotesService(INotesRepository repository) : INotesService
{
  private readonly INotesRepository _repository = repository;
  public async Task<ServiceResponse<NoteToGetDto>> CreateNoteAsync(int userId, NoteToCreateDto note)
  {
    if (userId <= 0)
    {
      return ServiceResponse<NoteToGetDto>.Failure(Error.Unauthorized("Notes.Unauthorized", "User not authorized"));
    }

    if (string.IsNullOrEmpty(note.Title) && string.IsNullOrEmpty(note.Body))
    {
      return ServiceResponse<NoteToGetDto>.Failure(Error.Validation("Notes.InvalidInput", "Note should have at least a title or body"));
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

    return ServiceResponse<NoteToGetDto>.Failure(Error.Failure("Notes.CreationFailed", "Failed to create note"));
  }

  public async Task<ServiceResponse<bool>> DeleteNoteAsync(int userId, int noteId)
  {
    if (userId <= 0)
    {
      return ServiceResponse<bool>.Failure(Error.Unauthorized("Notes.Unauthorized", "User not authorized"));
    }

    var request = new DeleteNoteRequest(noteId, userId!);

    var success = await _repository.DeleteNoteAsync(request);

    if (!success)
    {
      return ServiceResponse<bool>.Failure(Error.Failure("Notes.DeletionFailed", "Failed to delete note"));
    }

    return ServiceResponse<bool>.Success(true);
  }

  public async Task<ServiceResponse<NoteToGetDto>> GetNoteByIdAsync(int userId, int noteId)
  {
    if (userId <= 0)
    {
      return ServiceResponse<NoteToGetDto>.Failure(Error.Unauthorized("Notes.Unauthorized", "User not authorized"));
    }

    var request = new GetNoteRequest(noteId, userId!);

    Note? note = await _repository.GetNoteByIdAsync(request);

    if (note == null)
    {
      return ServiceResponse<NoteToGetDto>.Failure(Error.NotFound("Notes.NotFound", "Note not found"));
    }

    var noteToGetDto = new NoteToGetDto
    {
      NoteId = note.NoteId,
      Title = note.Title,
      Body = note.Body,
      CreatedAt = note.CreatedAt,
      UpdatedAt = note.UpdatedAt
    };

    return ServiceResponse<NoteToGetDto>.Success(noteToGetDto);
  }

  public async Task<ServiceResponse<IEnumerable<NoteToGetDto>>> GetNotesAsync(int userId)
  {
    if (userId <= 0)
    {
      return ServiceResponse<IEnumerable<NoteToGetDto>>.Failure(Error.Unauthorized("Notes.Unauthorized", "User not authorized"));
    }

    IEnumerable<Note> notes = await _repository.GetNotesByUserIdAsync(userId!);

    var notesToGetDto = notes.Select(note => new NoteToGetDto
    {
      NoteId = note.NoteId,
      Title = note.Title,
      Body = note.Body,
      CreatedAt = note.CreatedAt,
      UpdatedAt = note.UpdatedAt
    });

    return ServiceResponse<IEnumerable<NoteToGetDto>>.Success(notesToGetDto);
  }

  public async Task<ServiceResponse<NoteToGetDto>> UpdateNoteAsync(int userId, int noteId, NoteToUpdateDto note)
  {
    if (userId <= 0)
    {
      return ServiceResponse<NoteToGetDto>.Failure(Error.Unauthorized("Notes.Unauthorized", "User not authorized"));
    }

    if (noteId <= 0)
    {
      return ServiceResponse<NoteToGetDto>.Failure(Error.Validation("Notes.InvalidInput", "Invalid note ID"));
    }

    if (string.IsNullOrWhiteSpace(note.Title) && string.IsNullOrWhiteSpace(note.Body))
    {
      return ServiceResponse<NoteToGetDto>.Failure(Error.Validation("Notes.InvalidInput", "At least title or body must be provided"));
    }

    var request = new UpdateNoteRequest(noteId, userId!, note.Title, note.Body);

    Note? updatedNote = await _repository.UpdateNoteAsync(request);

    if (updatedNote == null)
    {
      return ServiceResponse<NoteToGetDto>.Failure(Error.Failure("Notes.UpdateFailed", "Failed to update note"));
    }
    var noteToGetDto = new NoteToGetDto
    {
      NoteId = updatedNote.NoteId,
      Title = updatedNote.Title,
      Body = updatedNote.Body,
      CreatedAt = updatedNote.CreatedAt,
      UpdatedAt = updatedNote.UpdatedAt
    };

    return ServiceResponse<NoteToGetDto>.Success(noteToGetDto);

  }
}
