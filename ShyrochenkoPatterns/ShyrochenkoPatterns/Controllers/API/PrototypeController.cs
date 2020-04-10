using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ShyrochenkoPatterns.Models.RequestModels.Posts;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using ShyrochenkoPatterns.ResourceLibrary;
using ShyrochenkoPatterns.Services.Interfaces.Prototype;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Controllers.API
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class PrototypeController : _BaseApiController
    {
        private IPostPrototype _postPrototype;

        public PrototypeController(IStringLocalizer<ErrorsResource> localizer, IPostPrototype postPrototype)
            : base(localizer)
        {
            _postPrototype = postPrototype;
        }

        [HttpPost("Poem")]
        public async Task<IActionResult> CreatePoem([FromBody]PostRequestModel model)
        {
            var response = await _postPrototype.CreatePoem(model);

            return Json(new JsonResponse<PostResponseModel>(response));
        }

        [HttpPost("Story")]
        public async Task<IActionResult> CreateStory([FromBody]PostRequestModel model)
        {
            var response = await _postPrototype.CreateStory(model);

            return Json(new JsonResponse<PostResponseModel>(response));
        }

        [HttpPost("Proverb")]
        public async Task<IActionResult> CreateProverb([FromBody]PostRequestModel model)
        {
            var response = await _postPrototype.CreateProverb(model);

            return Json(new JsonResponse<PostResponseModel>(response));
        }
    }
}
