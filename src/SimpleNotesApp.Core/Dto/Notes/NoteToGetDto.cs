namespace SimpleNotesApp.Core.Dto.Notes;


public partial class NoteToGetDto
{
  public int NoteId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Body { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}
