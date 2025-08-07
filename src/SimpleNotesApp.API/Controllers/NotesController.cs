using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleNotesApp.Core.Dto.Notes;
using SimpleNotesApp.Core.Services;

namespace SimpleNotesApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotesController(INotesService service) : ControllerBase
{
  private readonly INotesService _service = service;

  [HttpPost]
  public async Task<IActionResult> CreateNote(NoteToCreateDto note)
  {
    var userId = User.FindFirst("userId")?.Value;
    var result = await _service.CreateNoteAsync(userId, note);

    return result.When(
        onSuccess: createdNote => Created($"/api/notes/{createdNote.NoteId}", createdNote),
        onFailure: BadRequest
    );
  }

  [HttpDelete("{noteId}")]
  public async Task<IActionResult> DeleteNote(int noteId)
  {
    var userId = User.FindFirst("userId")?.Value;
    var result = await _service.DeleteNoteAsync(userId, noteId);

    return result.When(
        onSuccess: _ => Ok(),
        onFailure: BadRequest
    );
  }

  [HttpGet("{noteId}")]
  public async Task<IActionResult> GetNoteById(int noteId)
  {
    var userId = User.FindFirst("userId")?.Value;
    var result = await _service.GetNoteByIdAsync(userId, noteId);

    return result.When(
        onSuccess: note => Ok(note),
        onFailure: BadRequest
    );
  }

  [HttpGet]
  public async Task<IActionResult> GetNotes()
  {
    var userId = User.FindFirst("userId")?.Value;
    var result = await _service.GetNotesAsync(userId);

    return result.When(
        onSuccess: notes => Ok(notes),
        onFailure: BadRequest
    );
  }

  [HttpPatch("{noteId}")]
  public async Task<IActionResult> UpdateNote(int noteId, NoteToUpdateDto note)
  {
    var userId = User.FindFirst("userId")?.Value;
    var result = await _service.UpdateNoteAsync(userId, noteId, note);

    return result.When(
        onSuccess: updatedNote => Ok(updatedNote),
        onFailure: BadRequest
    );
  }
}
