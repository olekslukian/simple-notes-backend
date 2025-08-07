using Microsoft.IdentityModel.Tokens;
using SimpleNotesApp.Core.Common;
using SimpleNotesApp.Core.Dto.Notes;
using SimpleNotesApp.Infrastructure.Models;
using SimpleNotesApp.Infrastructure.Repositories;
using SimpleNotesApp.Infrastructure.Repositories.Requests;

namespace SimpleNotesApp.Core.Services;

public class NotesService(INotesRepository repository) : INotesService
{
  private readonly INotesRepository _repository = repository;
  public async Task<ServiceResponse<NoteToGetDto>> CreateNoteAsync(string? userId, NoteToCreateDto note)
  {
    if (string.IsNullOrEmpty(userId))
    {
      return ServiceResponse<NoteToGetDto>.Failure("User not authorized");
    }

    if (string.IsNullOrEmpty(note.Title) && string.IsNullOrEmpty(note.Body))
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

  public async Task<ServiceResponse<bool>> DeleteNoteAsync(string? userId, int noteId)
  {
    if (string.IsNullOrEmpty(userId))
    {
      return ServiceResponse<bool>.Failure("User not authorized");
    }

    var request = new DeleteNoteRequest(noteId, userId!);

    var success = await _repository.DeleteNoteAsync(request);

    if (!success)
    {
      return ServiceResponse<bool>.Failure("Failed to delete note");
    }

    return ServiceResponse<bool>.Success(true);
  }

  public async Task<ServiceResponse<NoteToGetDto>> GetNoteByIdAsync(string? userId, int noteId)
  {
    if (string.IsNullOrEmpty(userId))
    {
      return ServiceResponse<NoteToGetDto>.Failure("User not authorized");
    }

    var request = new GetNoteRequest(noteId, userId!);

    Note? note = await _repository.GetNoteByIdAsync(request);

    if (note == null)
    {
      return ServiceResponse<NoteToGetDto>.Failure("Note not found");
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

  public async Task<ServiceResponse<IEnumerable<NoteToGetDto>>> GetNotesAsync(string? userId)
  {
    if (string.IsNullOrEmpty(userId))
    {
      return ServiceResponse<IEnumerable<NoteToGetDto>>.Failure("User not authorized");
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

  public async Task<ServiceResponse<NoteToGetDto>> UpdateNoteAsync(string? userId, int noteId, NoteToUpdateDto note)
  {
    if (string.IsNullOrEmpty(userId))
    {
      return ServiceResponse<NoteToGetDto>.Failure("User not authorized");
    }

    if (noteId <= 0)
    {
      return ServiceResponse<NoteToGetDto>.Failure("Invalid note ID");
    }

    if (string.IsNullOrWhiteSpace(note.Title) && string.IsNullOrWhiteSpace(note.Body))
    {
      return ServiceResponse<NoteToGetDto>.Failure("At least title or body must be provided");
    }

    var request = new UpdateNoteRequest(noteId, userId!, note.Title, note.Body);

    Note? updatedNote = await _repository.UpdateNoteAsync(request);

    if (updatedNote == null)
    {
      return ServiceResponse<NoteToGetDto>.Failure("Failed to update note");
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
