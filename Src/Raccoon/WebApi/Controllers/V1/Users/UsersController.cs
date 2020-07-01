using Application.V1.Users;
using Domain.Models.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Controllers.V1.Users
{
    /// <summary>
    /// Represents a RESTful user service.
    /// </summary>
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class UsersController : BaseController
    {
        /// <summary>
        /// Sends sms code to supplied mobile number
        /// </summary>
        /// <param name="command">User data to send code</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        /// <response code="200">Sms code sent successfully</response>
        /// <response code="400">Validation error</response>
        /// <response code="404">User not found with specified name</response>
        /// <response code="500">Sms service not available or couldn't save user</response>
        [HttpPost(ApiRoutes.Users.SendCode)]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> SendCode(SendCode.SendCodeCommand command, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Authenticates user
        /// </summary>
        /// <param name="command">Login credentials</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Json web token and refresh token</returns>
        /// <response code="200">Login process past successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost(ApiRoutes.Users.Login)]
        [ProducesResponseType(typeof(Login.AuthenticationResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Login(Login.LoginCommand command, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Updates json web token
        /// </summary>
        /// <param name="command">Previous tokens</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Json web token and refresh token</returns>
        /// <response code="200">Auth process past successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost(ApiRoutes.Users.RefreshToken)]
        [ProducesResponseType(typeof(Login.AuthenticationResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> RefreshToken(RefreshToken.RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(command, cancellationToken));
        }
    }
}
