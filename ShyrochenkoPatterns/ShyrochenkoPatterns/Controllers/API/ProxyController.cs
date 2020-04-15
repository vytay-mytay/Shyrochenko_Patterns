using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using ShyrochenkoPatterns.ResourceLibrary;
using ShyrochenkoPatterns.Services.Interfaces.Proxy;
using System.Threading.Tasks;

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

        [HttpPatch("Like/{id}")]
        public async Task<IActionResult> SetLike(int id)
        {
            await _proxy.SetLike(id);

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("Set - done")));
        }

        [HttpGet("Read/{id}")]
        public async Task<IActionResult> Read(int id)
        {
            var response = await _proxy.Read(id);

            return Json(new JsonResponse<PostResponseModel>(response));
        }
    }
}