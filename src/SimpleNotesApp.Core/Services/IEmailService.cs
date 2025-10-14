using System;
using SimpleNotesApp.Core.Common;

namespace SimpleNotesApp.Core.Services;

public interface IEmailService
{
  Task<ServiceResponse<bool>> SendTestEmailAsync(string to);
  Task<ServiceResponse<bool>> SendVerificationEmailAsync(string to, string otpCode);
}
