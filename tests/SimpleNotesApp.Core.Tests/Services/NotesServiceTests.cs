using FluentAssertions;
using Moq;
using SimpleNotesApp.Core.Common;
using SimpleNotesApp.Core.Dto.Notes;
using SimpleNotesApp.Core.Models;
using SimpleNotesApp.Core.Repositories;
using SimpleNotesApp.Core.Repositories.Requests;
using SimpleNotesApp.Core.Services;

namespace SimpleNotesApp.Core.Tests.Services;

public class NotesServiceTests
{
  private readonly Mock<INotesRepository> _notesRepository;
  private readonly NotesService _notesService;

  public NotesServiceTests()
  {
    _notesRepository = new Mock<INotesRepository>();
    _notesService = new NotesService(_notesRepository.Object);
  }

  #region CreateNoteAsync Tests

  [Fact]
  public async Task CreateNoteAsync_ValidNote_ReturnsSuccessWithNoteDto()
  {
    var userId = 1;
    var noteToCreate = new NoteToCreateDto
    {
      Title = "Test Note",
      Body = "Test Body"
    };

    var createdNote = new Note
    {
      NoteId = 1,
      UserId = userId,
      Title = noteToCreate.Title,
      Body = noteToCreate.Body,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    _notesRepository.Setup(r => r.CreateNoteAsync(It.IsAny<CreateNoteRequest>()))
      .ReturnsAsync(createdNote);

    var result = await _notesService.CreateNoteAsync(userId, noteToCreate);

    result.IsSuccess.Should().BeTrue();
    result.Data.Should().NotBeNull();
    result.Data!.NoteId.Should().Be(1);
    result.Data.Title.Should().Be("Test Note");
    result.Data.Body.Should().Be("Test Body");
  }

  [Fact]
  public async Task CreateNoteAsync_OnlyTitle_ReturnsSuccess()
  {
    var userId = 1;
    var noteToCreate = new NoteToCreateDto
    {
      Title = "Only Title",
      Body = ""
    };

    var createdNote = new Note
    {
      NoteId = 2,
      UserId = userId,
      Title = noteToCreate.Title,
      Body = noteToCreate.Body,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    _notesRepository.Setup(r => r.CreateNoteAsync(It.IsAny<CreateNoteRequest>()))
      .ReturnsAsync(createdNote);

    var result = await _notesService.CreateNoteAsync(userId, noteToCreate);

    result.IsSuccess.Should().BeTrue();
    result.Data!.Title.Should().Be("Only Title");
  }

  [Fact]
  public async Task CreateNoteAsync_OnlyBody_ReturnsSuccess()
  {
    var userId = 1;
    var noteToCreate = new NoteToCreateDto
    {
      Title = "",
      Body = "Only Body"
    };

    var createdNote = new Note
    {
      NoteId = 3,
      UserId = userId,
      Title = noteToCreate.Title,
      Body = noteToCreate.Body,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    _notesRepository.Setup(r => r.CreateNoteAsync(It.IsAny<CreateNoteRequest>()))
      .ReturnsAsync(createdNote);

    var result = await _notesService.CreateNoteAsync(userId, noteToCreate);

    result.IsSuccess.Should().BeTrue();
    result.Data!.Body.Should().Be("Only Body");
  }

  [Fact]
  public async Task CreateNoteAsync_EmptyTitleAndBody_ReturnsValidationError()
  {
    var userId = 1;
    var noteToCreate = new NoteToCreateDto
    {
      Title = "",
      Body = ""
    };

    var result = await _notesService.CreateNoteAsync(userId, noteToCreate);

    result.IsSuccess.Should().BeFalse();
    result.Error.Should().NotBeNull();
    result.Error!.Type.Should().Be(ErrorType.Validation);
    result.Error.Code.Should().Be("Notes.InvalidInput");
    _notesRepository.Verify(r => r.CreateNoteAsync(It.IsAny<CreateNoteRequest>()), Times.Never);
  }

  [Fact]
  public async Task CreateNoteAsync_InvalidUserId_ReturnsUnauthorizedError()
  {
    var noteToCreate = new NoteToCreateDto
    {
      Title = "Test",
      Body = "Test"
    };

    var result = await _notesService.CreateNoteAsync(0, noteToCreate);

    result.IsSuccess.Should().BeFalse();
    result.Error!.Type.Should().Be(ErrorType.Unauthorized);
    result.Error.Code.Should().Be("Notes.Unauthorized");
    _notesRepository.Verify(r => r.CreateNoteAsync(It.IsAny<CreateNoteRequest>()), Times.Never);
  }

  [Fact]
  public async Task CreateNoteAsync_RepositoryReturnsNull_ReturnsFailureError()
  {
    var userId = 1;
    var noteToCreate = new NoteToCreateDto
    {
      Title = "Test",
      Body = "Test"
    };

    _notesRepository.Setup(r => r.CreateNoteAsync(It.IsAny<CreateNoteRequest>()))
      .ReturnsAsync((Note?)null);

    var result = await _notesService.CreateNoteAsync(userId, noteToCreate);

    result.IsSuccess.Should().BeFalse();
    result.Error!.Type.Should().Be(ErrorType.Failure);
    result.Error.Code.Should().Be("Notes.CreationFailed");
  }

  #endregion

  #region GetNoteByIdAsync Tests

  [Fact]
  public async Task GetNoteByIdAsync_ExistingNote_ReturnsNoteDto()
  {
    var userId = 1;
    var noteId = 1;
    var note = new Note
    {
      NoteId = noteId,
      UserId = userId,
      Title = "Test Note",
      Body = "Test Body",
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    _notesRepository.Setup(r => r.GetNoteByIdAsync(It.IsAny<GetNoteRequest>()))
      .ReturnsAsync(note);

    var result = await _notesService.GetNoteByIdAsync(userId, noteId);

    result.IsSuccess.Should().BeTrue();
    result.Data.Should().NotBeNull();
    result.Data!.NoteId.Should().Be(noteId);
    result.Data.Title.Should().Be("Test Note");
    result.Data.Body.Should().Be("Test Body");
  }

  [Fact]
  public async Task GetNoteByIdAsync_NoteNotFound_ReturnsNotFoundError()
  {
    var userId = 1;
    var noteId = 999;

    _notesRepository.Setup(r => r.GetNoteByIdAsync(It.IsAny<GetNoteRequest>()))
      .ReturnsAsync((Note?)null);

    var result = await _notesService.GetNoteByIdAsync(userId, noteId);

    result.IsSuccess.Should().BeFalse();
    result.Error!.Type.Should().Be(ErrorType.NotFound);
    result.Error.Code.Should().Be("Notes.NotFound");
  }

  [Fact]
  public async Task GetNoteByIdAsync_InvalidUserId_ReturnsUnauthorizedError()
  {
    var result = await _notesService.GetNoteByIdAsync(0, 1);

    result.IsSuccess.Should().BeFalse();
    result.Error!.Type.Should().Be(ErrorType.Unauthorized);
    _notesRepository.Verify(r => r.GetNoteByIdAsync(It.IsAny<GetNoteRequest>()), Times.Never);
  }

  #endregion

  #region GetNotesAsync Tests

  [Fact]
  public async Task GetNotesAsync_UserHasNotes_ReturnsAllNotes()
  {
    var userId = 1;
    var notes = new List<Note>
    {
      new() { NoteId = 1, UserId = userId, Title = "Note 1", Body = "Body 1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
      new() { NoteId = 2, UserId = userId, Title = "Note 2", Body = "Body 2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
      new() { NoteId = 3, UserId = userId, Title = "Note 3", Body = "Body 3", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
    };

    _notesRepository.Setup(r => r.GetNotesByUserIdAsync(userId))
      .ReturnsAsync(notes);

    var result = await _notesService.GetNotesAsync(userId);

    result.IsSuccess.Should().BeTrue();
    result.Data.Should().NotBeNull();
    result.Data.Should().HaveCount(3);
    result.Data!.Select(n => n.NoteId).Should().ContainInOrder(1, 2, 3);
  }

  [Fact]
  public async Task GetNotesAsync_UserHasNoNotes_ReturnsEmptyList()
  {
    var userId = 1;
    var notes = new List<Note>();

    _notesRepository.Setup(r => r.GetNotesByUserIdAsync(userId))
      .ReturnsAsync(notes);

    var result = await _notesService.GetNotesAsync(userId);

    result.IsSuccess.Should().BeTrue();
    result.Data.Should().NotBeNull();
    result.Data.Should().BeEmpty();
  }

  [Fact]
  public async Task GetNotesAsync_InvalidUserId_ReturnsUnauthorizedError()
  {
    var result = await _notesService.GetNotesAsync(0);

    result.IsSuccess.Should().BeFalse();
    result.Error!.Type.Should().Be(ErrorType.Unauthorized);
    _notesRepository.Verify(r => r.GetNotesByUserIdAsync(It.IsAny<int>()), Times.Never);
  }

  #endregion

  #region UpdateNoteAsync Tests

  [Fact]
  public async Task UpdateNoteAsync_ValidUpdate_ReturnsUpdatedNote()
  {
    var userId = 1;
    var noteId = 1;
    var noteToUpdate = new NoteToUpdateDto
    {
      Title = "Updated Title",
      Body = "Updated Body"
    };

    var updatedNote = new Note
    {
      NoteId = noteId,
      UserId = userId,
      Title = noteToUpdate.Title!,
      Body = noteToUpdate.Body!,
      CreatedAt = DateTime.UtcNow.AddDays(-1),
      UpdatedAt = DateTime.UtcNow
    };

    _notesRepository.Setup(r => r.UpdateNoteAsync(It.IsAny<UpdateNoteRequest>()))
      .ReturnsAsync(updatedNote);

    var result = await _notesService.UpdateNoteAsync(userId, noteId, noteToUpdate);

    result.IsSuccess.Should().BeTrue();
    result.Data.Should().NotBeNull();
    result.Data!.Title.Should().Be("Updated Title");
    result.Data.Body.Should().Be("Updated Body");
  }

  [Fact]
  public async Task UpdateNoteAsync_OnlyTitleUpdated_ReturnsSuccess()
  {
    var userId = 1;
    var noteId = 1;
    var noteToUpdate = new NoteToUpdateDto
    {
      Title = "Updated Title Only",
      Body = null
    };

    var updatedNote = new Note
    {
      NoteId = noteId,
      UserId = userId,
      Title = noteToUpdate.Title,
      Body = "Original Body",
      CreatedAt = DateTime.UtcNow.AddDays(-1),
      UpdatedAt = DateTime.UtcNow
    };

    _notesRepository.Setup(r => r.UpdateNoteAsync(It.IsAny<UpdateNoteRequest>()))
      .ReturnsAsync(updatedNote);

    var result = await _notesService.UpdateNoteAsync(userId, noteId, noteToUpdate);

    result.IsSuccess.Should().BeTrue();
    result.Data!.Title.Should().Be("Updated Title Only");
  }

  [Fact]
  public async Task UpdateNoteAsync_EmptyTitleAndBody_ReturnsValidationError()
  {
    var userId = 1;
    var noteId = 1;
    var noteToUpdate = new NoteToUpdateDto
    {
      Title = "",
      Body = ""
    };

    var result = await _notesService.UpdateNoteAsync(userId, noteId, noteToUpdate);

    result.IsSuccess.Should().BeFalse();
    result.Error!.Type.Should().Be(ErrorType.Validation);
    result.Error.Code.Should().Be("Notes.InvalidInput");
    _notesRepository.Verify(r => r.UpdateNoteAsync(It.IsAny<UpdateNoteRequest>()), Times.Never);
  }

  [Fact]
  public async Task UpdateNoteAsync_InvalidUserId_ReturnsUnauthorizedError()
  {
    var noteToUpdate = new NoteToUpdateDto { Title = "Test" };

    var result = await _notesService.UpdateNoteAsync(0, 1, noteToUpdate);

    result.IsSuccess.Should().BeFalse();
    result.Error!.Type.Should().Be(ErrorType.Unauthorized);
    _notesRepository.Verify(r => r.UpdateNoteAsync(It.IsAny<UpdateNoteRequest>()), Times.Never);
  }

  [Fact]
  public async Task UpdateNoteAsync_InvalidNoteId_ReturnsValidationError()
  {
    var noteToUpdate = new NoteToUpdateDto { Title = "Test" };

    var result = await _notesService.UpdateNoteAsync(1, 0, noteToUpdate);

    result.IsSuccess.Should().BeFalse();
    result.Error!.Type.Should().Be(ErrorType.Validation);
    result.Error.Code.Should().Be("Notes.InvalidInput");
    _notesRepository.Verify(r => r.UpdateNoteAsync(It.IsAny<UpdateNoteRequest>()), Times.Never);
  }

  [Fact]
  public async Task UpdateNoteAsync_RepositoryReturnsNull_ReturnsFailureError()
  {
    var userId = 1;
    var noteId = 1;
    var noteToUpdate = new NoteToUpdateDto { Title = "Test" };

    _notesRepository.Setup(r => r.UpdateNoteAsync(It.IsAny<UpdateNoteRequest>()))
      .ReturnsAsync((Note?)null);

    var result = await _notesService.UpdateNoteAsync(userId, noteId, noteToUpdate);

    result.IsSuccess.Should().BeFalse();
    result.Error!.Type.Should().Be(ErrorType.Failure);
    result.Error.Code.Should().Be("Notes.UpdateFailed");
  }

  #endregion

  #region DeleteNoteAsync Tests

  [Fact]
  public async Task DeleteNoteAsync_ExistingNote_ReturnsSuccess()
  {
    var userId = 1;
    var noteId = 1;

    _notesRepository.Setup(r => r.DeleteNoteAsync(It.IsAny<DeleteNoteRequest>()))
      .ReturnsAsync(true);

    var result = await _notesService.DeleteNoteAsync(userId, noteId);

    result.IsSuccess.Should().BeTrue();
    result.Data.Should().BeTrue();
  }

  [Fact]
  public async Task DeleteNoteAsync_RepositoryReturnsFalse_ReturnsFailureError()
  {
    var userId = 1;
    var noteId = 1;

    _notesRepository.Setup(r => r.DeleteNoteAsync(It.IsAny<DeleteNoteRequest>()))
      .ReturnsAsync(false);

    var result = await _notesService.DeleteNoteAsync(userId, noteId);

    result.IsSuccess.Should().BeFalse();
    result.Error!.Type.Should().Be(ErrorType.Failure);
    result.Error.Code.Should().Be("Notes.DeletionFailed");
  }

  [Fact]
  public async Task DeleteNoteAsync_InvalidUserId_ReturnsUnauthorizedError()
  {
    var result = await _notesService.DeleteNoteAsync(0, 1);

    result.IsSuccess.Should().BeFalse();
    result.Error!.Type.Should().Be(ErrorType.Unauthorized);
    _notesRepository.Verify(r => r.DeleteNoteAsync(It.IsAny<DeleteNoteRequest>()), Times.Never);
  }

  #endregion
}
