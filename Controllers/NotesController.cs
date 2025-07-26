using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleNotesApp.Dto.Notes;
using SimpleNotesApp.Services;

namespace SimpleNotesApp.Controllers;

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
        onSuccess: createdNote => CreatedAtAction(nameof(CreateNote), new { id = createdNote.NoteId }, createdNote),
        onFailure: BadRequest
    );
  }
}
