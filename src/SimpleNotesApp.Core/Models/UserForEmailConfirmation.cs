using System;

namespace SimpleNotesApp.Core.Models;

public partial class UserForEmailConfirmation
{
  public int UserId { get; set; }
  public string Email { get; set; } = string.Empty;
  public byte[] OtpHash { get; set; } = [];
  public byte[] OtpSalt { get; set; } = [];
  public DateTime OtpExpiresAt { get; set; }
}
