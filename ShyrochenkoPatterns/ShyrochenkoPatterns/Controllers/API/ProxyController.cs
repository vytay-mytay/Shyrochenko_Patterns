using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ShyrochenkoPatterns.ResourceLibrary;
using ShyrochenkoPatterns.Services.Interfaces.Proxy;

namespace ShyrochenkoPatterns.Controllers.API
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class ProxyController : _BaseApiController
    {
        private IProxy _proxy;

        public ProxyController(IStringLocalizer<ErrorsResource> errorsLocalizer, IProxy proxy)
            : base(errorsLocalizer)
        {
            _proxy = proxy;
        }


    }
}