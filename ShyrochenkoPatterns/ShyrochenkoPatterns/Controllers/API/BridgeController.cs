using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ShyrochenkoPatterns.ResourceLibrary;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Controllers.API
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class BridgeController : _BaseApiController
    {
        public BridgeController(IStringLocalizer<ErrorsResource> errorsLocalizer)
            : base(errorsLocalizer)
        {
        }

        //[HttpPost("Vehicle")]
        //public async Task<IActionResult> Create([FromQuery] )
        //{
        //
        //}

    }
}