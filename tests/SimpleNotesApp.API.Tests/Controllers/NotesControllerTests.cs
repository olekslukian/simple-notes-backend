using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleNotesApp.API.Controllers;
using SimpleNotesApp.Core.Common;
using SimpleNotesApp.Core.Dto.Notes;
using SimpleNotesApp.Core.Services;
using System.Security.Claims;

namespace SimpleNotesApp.API.Tests.Controllers;

public class NotesControllerTests
{
  private readonly Mock<INotesService> _notesService;
  private readonly NotesController _controller;

  public NotesControllerTests()
  {
    _notesService = new Mock<INotesService>();
    _controller = new NotesController(_notesService.Object);

    var claims = new List<Claim>
    {
      new("userId", "1")
    };
    var identity = new ClaimsIdentity(claims, "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext { User = claimsPrincipal }
    };
  }

  #region CreateNote Tests

  [Fact]
  public async Task CreateNote_ValidNote_ReturnsCreatedWithLocation()
  {
    var noteToCreate = new NoteToCreateDto
    {
      Title = "Test Note",
      Body = "Test Body"
    };

    var createdNote = new NoteToGetDto
    {
      NoteId = 1,
      Title = noteToCreate.Title,
      Body = noteToCreate.Body,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    _notesService.Setup(s => s.CreateNoteAsync(1, noteToCreate))
      .ReturnsAsync(ServiceResponse<NoteToGetDto>.Success(createdNote));

    var result = await _controller.CreateNote(noteToCreate);

    var createdResult = result.Should().BeOfType<CreatedResult>().Subject;
    createdResult.StatusCode.Should().Be(201);
    createdResult.Location.Should().Be("/api/notes/1");
    createdResult.Value.Should().BeEquivalentTo(createdNote);
  }

  [Fact]
  public async Task CreateNote_EmptyTitleAndBody_ReturnsBadRequest()
  {
    var noteToCreate = new NoteToCreateDto
    {
      Title = "",
      Body = ""
    };

    var error = Error.Validation("Notes.InvalidInput", "Note should have at least a title or body");

    _notesService.Setup(s => s.CreateNoteAsync(1, noteToCreate))
      .ReturnsAsync(ServiceResponse<NoteToGetDto>.Failure(error));

    var result = await _controller.CreateNote(noteToCreate);

    var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
    var problemDetails = objectResult.Value.Should().BeAssignableTo<ProblemDetails>().Subject;
    problemDetails.Status.Should().Be(400);
  }

  #endregion

  #region GetNoteById Tests

  [Fact]
  public async Task GetNoteById_ExistingNote_ReturnsOkWithNote()
  {
    var noteId = 1;
    var note = new NoteToGetDto
    {
      NoteId = noteId,
      Title = "Test Note",
      Body = "Test Body",
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    _notesService.Setup(s => s.GetNoteByIdAsync(1, noteId))
      .ReturnsAsync(ServiceResponse<NoteToGetDto>.Success(note));

    var result = await _controller.GetNoteById(noteId);

    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    okResult.Value.Should().BeEquivalentTo(note);
  }

  [Fact]
  public async Task GetNoteById_NonExistingNote_ReturnsNotFound()
  {
    var noteId = 999;
    var error = Error.NotFound("Notes.NotFound", "Note not found");

    _notesService.Setup(s => s.GetNoteByIdAsync(1, noteId))
      .ReturnsAsync(ServiceResponse<NoteToGetDto>.Failure(error));

    var result = await _controller.GetNoteById(noteId);

    var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
    objectResult.StatusCode.Should().Be(404);
  }

  #endregion

  #region GetNotes Tests

  [Fact]
  public async Task GetNotes_UserHasNotes_ReturnsOkWithNotes()
  {
    var notes = new List<NoteToGetDto>
    {
      new() { NoteId = 1, Title = "Note 1", Body = "Body 1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
      new() { NoteId = 2, Title = "Note 2", Body = "Body 2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
    };

    _notesService.Setup(s => s.GetNotesAsync(1))
      .ReturnsAsync(ServiceResponse<IEnumerable<NoteToGetDto>>.Success(notes));

    var result = await _controller.GetNotes();

    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    var returnedNotes = okResult.Value.Should().BeAssignableTo<IEnumerable<NoteToGetDto>>().Subject;
    returnedNotes.Should().HaveCount(2);
  }

  [Fact]
  public async Task GetNotes_UserHasNoNotes_ReturnsOkWithEmptyList()
  {
    var notes = new List<NoteToGetDto>();

    _notesService.Setup(s => s.GetNotesAsync(1))
      .ReturnsAsync(ServiceResponse<IEnumerable<NoteToGetDto>>.Success(notes));

    var result = await _controller.GetNotes();

    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    var returnedNotes = okResult.Value.Should().BeAssignableTo<IEnumerable<NoteToGetDto>>().Subject;
    returnedNotes.Should().BeEmpty();
  }

  #endregion

  #region UpdateNote Tests

  [Fact]
  public async Task UpdateNote_ValidUpdate_ReturnsOkWithUpdatedNote()
  {
    var noteId = 1;
    var noteToUpdate = new NoteToUpdateDto
    {
      Title = "Updated Title",
      Body = "Updated Body"
    };

    var updatedNote = new NoteToGetDto
    {
      NoteId = noteId,
      Title = noteToUpdate.Title!,
      Body = noteToUpdate.Body!,
      CreatedAt = DateTime.UtcNow.AddDays(-1),
      UpdatedAt = DateTime.UtcNow
    };

    _notesService.Setup(s => s.UpdateNoteAsync(1, noteId, noteToUpdate))
      .ReturnsAsync(ServiceResponse<NoteToGetDto>.Success(updatedNote));

    var result = await _controller.UpdateNote(noteId, noteToUpdate);

    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    okResult.Value.Should().BeEquivalentTo(updatedNote);
  }

  [Fact]
  public async Task UpdateNote_InvalidNoteId_ReturnsBadRequest()
  {
    var noteId = 0;
    var noteToUpdate = new NoteToUpdateDto { Title = "Test" };

    var error = Error.Validation("Notes.InvalidInput", "Invalid note ID");

    _notesService.Setup(s => s.UpdateNoteAsync(1, noteId, noteToUpdate))
      .ReturnsAsync(ServiceResponse<NoteToGetDto>.Failure(error));

    var result = await _controller.UpdateNote(noteId, noteToUpdate);

    var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
    var problemDetails = objectResult.Value.Should().BeAssignableTo<ProblemDetails>().Subject;
    problemDetails.Status.Should().Be(400);
  }

  [Fact]
  public async Task UpdateNote_NonExistingNote_ReturnsNotFoundOrFailure()
  {
    var noteId = 999;
    var noteToUpdate = new NoteToUpdateDto { Title = "Test" };

    var error = Error.Failure("Notes.UpdateFailed", "Failed to update note");

    _notesService.Setup(s => s.UpdateNoteAsync(1, noteId, noteToUpdate))
      .ReturnsAsync(ServiceResponse<NoteToGetDto>.Failure(error));

    var result = await _controller.UpdateNote(noteId, noteToUpdate);

    var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
    objectResult.StatusCode.Should().Be(500);
  }

  #endregion

  #region DeleteNote Tests

  [Fact]
  public async Task DeleteNote_ExistingNote_ReturnsOk()
  {
    var noteId = 1;

    _notesService.Setup(s => s.DeleteNoteAsync(1, noteId))
      .ReturnsAsync(ServiceResponse<bool>.Success(true));

    var result = await _controller.DeleteNote(noteId);

    result.Should().BeOfType<OkResult>();
  }

  [Fact]
  public async Task DeleteNote_NonExistingNote_ReturnsFailure()
  {
    var noteId = 999;
    var error = Error.Failure("Notes.DeletionFailed", "Failed to delete note");

    _notesService.Setup(s => s.DeleteNoteAsync(1, noteId))
      .ReturnsAsync(ServiceResponse<bool>.Failure(error));

    var result = await _controller.DeleteNote(noteId);

    var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
    objectResult.StatusCode.Should().Be(500);
  }

  #endregion
}
