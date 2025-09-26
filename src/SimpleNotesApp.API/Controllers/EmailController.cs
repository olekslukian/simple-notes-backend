using Microsoft.AspNetCore.Mvc;
using SimpleNotesApp.Core.Services;

namespace SimpleNotesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController(IEmailService emailService) : BaseController
    {
        private readonly IEmailService _emailService = emailService;

        [HttpPost("send-test-email")]
        public async Task<IActionResult> SendTestEmail([FromBody] string email)
        {
            var result = await _emailService.SendTestEmailAsync(email);

            return result.When(
                onSuccess: _ => Ok("Test email sent successfully"),
                onFailure: Problem
            );
        }
    }
}
