using System;
using SimpleNotesApp.Core.Common;

namespace SimpleNotesApp.Core.Services;

public interface IEmailService
{
  Task<ServiceResponse<string>> SendTestEmailAsync(string to);
  Task<ServiceResponse<string>> SendVerificationEmailAsync(string to, string otpCode);
}
