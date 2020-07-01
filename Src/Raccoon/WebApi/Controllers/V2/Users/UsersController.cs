using Application.V1.Users;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace WebApi.Controllers.V2.Users
{
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class UsersController : BaseController
    {
        [HttpPost(ApiRoutes.Users.SendCode)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult SendCode(SendCode.SendCodeCommand command, CancellationToken cancellationToken)
        {
            return Ok();
        }
    }
}
