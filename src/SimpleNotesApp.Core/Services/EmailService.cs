using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleNotesApp.Core.Common;
using SimpleNotesApp.Core.Dto;

namespace SimpleNotesApp.Core.Services;

public class EmailService : IEmailService
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<EmailService> _logger;
  private readonly string _apiKey;
  private readonly string _domain;
  private readonly string _fromEmail;

  public EmailService(HttpClient httpClient, ILogger<EmailService> logger, IConfiguration config)
  {
    _httpClient = httpClient;
    _logger = logger;

    _apiKey = config["Mailgun:ApiKey"] ?? throw new InvalidOperationException("Mailgun API key is not configured");
    _domain = config["Mailgun:SandboxDomain"] ?? throw new InvalidOperationException("Mailgun Domain is not configured");
    _fromEmail = config["Mailgun:SandboxFromEmail"] ?? throw new InvalidOperationException("Mailgun FromEmail is not configured");

    var authValue = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("api:" + _apiKey));
    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authValue);
  }

  public async Task<ServiceResponse<bool>> SendTestEmailAsync(string to)
  {

    return await SendEmailInternalAsync(
      to: to,
      subject: "Hello from SimpleNotesApp",
      body: "This is a test email sent from SimpleNotesApp using Mailgun.",
      htmlBody: null,
      from: null
    );
  }

  public async Task<ServiceResponse<bool>> SendVerificationEmailAsync(string to)
  {
    throw new NotImplementedException();
  }

  private async Task<ServiceResponse<bool>> SendEmailInternalAsync(string to, string subject, string body, string? htmlBody, string? from)
  {
    try
    {
      var formData = new List<KeyValuePair<string, string>>
      {
        new("from", from ?? "SimpleNotesApp <" + _fromEmail + ">"),
        new("to", to),
        new("subject", subject),
        new("text", body)
      };

      if (!string.IsNullOrEmpty(htmlBody))
      {
        formData.Add(new("html", htmlBody));
      }

      var content = new FormUrlEncodedContent(formData);
      var url = Constants.MailgunApiBaseUrl + $"/{_domain}/messages";

      var response = await _httpClient.PostAsync(url, content);

      if (response.IsSuccessStatusCode)
      {
        _logger.LogInformation("Email successfully sent");

        return ServiceResponse<bool>.Success(true);
      }
      else
      {
        _logger.LogError("Failed to send email");

        return ServiceResponse<bool>.Failure(Error.Failure("Email.SendFailed", "Failed to send email"));
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to send email");

      return ServiceResponse<bool>.Failure(Error.Failure("Email.SendFailed", "Failed to send email"));
    }
  }
}
