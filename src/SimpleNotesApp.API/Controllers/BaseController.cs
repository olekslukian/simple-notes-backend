using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SimpleNotesApp.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId");
            return int.TryParse(userIdClaim?.Value, out var userId) ? userId : 0;

        }
    }
}
