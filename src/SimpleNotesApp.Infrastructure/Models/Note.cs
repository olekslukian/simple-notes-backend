namespace SimpleNotesApp.Infrastructure.Models;

public partial class Note
{
  public int NoteId { get; set; }
  public int UserId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Body { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}
