using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using static Application.V1.Test.Dictionary;

namespace WebApi.Controllers.V1.Tests
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TestsController : BaseController
    {
        [HttpGet(ApiRoutes.Tests.Get)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new DictionaryQuery(), cancellationToken));
        }
    }
}
