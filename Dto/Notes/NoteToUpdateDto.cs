namespace SimpleNotesApp.Dto.Notes;

public partial class NoteToUpdateDto
{
  public int NoteId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Body { get; set; } = string.Empty;
}

