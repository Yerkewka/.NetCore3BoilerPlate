using Application.V1.Users;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Controllers.V1.Users
{
    [Produces("application/json")]
    public class UsersController : BaseController
    {
        [HttpPost(ApiRoutes.Users.SendCode)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SendCode(SendCode.SendCodeCommand command, CancellationToken cancellationToken)
        {
            var succeed = await Mediator.Send(command, cancellationToken);
            if (!succeed)
                return StatusCode(500);

            return Ok();
        }
    }
}
