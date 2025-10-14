using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleNotesApp.Core.Common;
using SimpleNotesApp.Core.Services;

namespace SimpleNotesApp.Infrastructure.Services;

public class MailgunEmailService : IEmailService
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<MailgunEmailService> _logger;
  private readonly string _apiBaseUrl;
  private readonly string _apiKey;
  private readonly string _domain;
  private readonly string _fromEmail;

  public MailgunEmailService(HttpClient httpClient, ILogger<MailgunEmailService> logger, IConfiguration config)
  {
    _httpClient = httpClient;
    _logger = logger;

    _apiBaseUrl = config["Mailgun:BaseUrl"] ?? throw new InvalidOperationException("Mailgun API base URL is not configured");
    _apiKey = config["Mailgun:ApiKey"] ?? throw new InvalidOperationException("Mailgun API key is not configured");
    _domain = config["Mailgun:Domain"] ?? throw new InvalidOperationException("Mailgun Domain is not configured");
    _fromEmail = config["Mailgun:FromEmail"] ?? throw new InvalidOperationException("Mailgun FromEmail is not configured");

    var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes("api:" + _apiKey));
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);
  }

  public async Task<ServiceResponse<bool>> SendTestEmailAsync(string to)
  {

    return await SendEmailInternalAsync(
      to: to,
      subject: "Hello from SimpleNotes",
      body: "This is a test email sent from SimpleNotesApp using Mailgun.",
      htmlBody: null,
      from: _fromEmail
    );
  }

  public async Task<ServiceResponse<bool>> SendVerificationEmailAsync(string to, string otpCode)
  {
    return await SendEmailInternalAsync(
      to: to,
      subject: "Your Verification Code for SimpleNotes",
      body: $"Your verification code is: {otpCode}",
      htmlBody: null,
      from: _fromEmail
    );
  }

  private async Task<ServiceResponse<bool>> SendEmailInternalAsync(string to, string subject, string body, string? htmlBody, string? from)
  {
    try
    {
      var formData = new List<KeyValuePair<string, string>>
      {
        new("from",  "SimpleNotes <" + _fromEmail + ">"),
        new("to", to),
        new("subject", subject),
        new("text", body)
      };

      if (!string.IsNullOrEmpty(htmlBody))
      {
        formData.Add(new("html", htmlBody));
      }

      var content = new FormUrlEncodedContent(formData);
      var url = _apiBaseUrl + $"/{_domain}/messages";

      var response = await _httpClient.PostAsync(url, content);

      if (response.IsSuccessStatusCode)
      {
        _logger.LogInformation("Email successfully sent");

        return ServiceResponse<bool>.Success(true);
      }
      else
      {
        var errorBody = await response.Content.ReadAsStringAsync();
        _logger.LogError("Failed to send email via Mailgun. Status: {StatusCode}. Response: {ErrorBody}", response.StatusCode, errorBody);

        var error = response.StatusCode switch
        {
          HttpStatusCode.Unauthorized => Error.Unauthorized("Email.AuthFailed", "Mailgun authentication failed"),
          HttpStatusCode.BadRequest => Error.Validation("Email.InvalidRequest", $"Mailgun rejected the request: {errorBody}"),
          _ => Error.Failure("Email.SendFailed", $"Mailgun API returned an error: {response.StatusCode}")
        };

        return ServiceResponse<bool>.Failure(error);
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to send email");

      return ServiceResponse<bool>.Failure(Error.Failure("Email.SendFailed", "Failed to send email"));
    }
  }
}
