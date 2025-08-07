using System;

namespace SimpleNotesApp.Infrastructure.Models;

public partial class UserForPasswordChange
{
  public int UserId { get; set; }
  public byte[] PasswordHash { get; set; } = [];
  public byte[] PasswordSalt { get; set; } = [];
}
