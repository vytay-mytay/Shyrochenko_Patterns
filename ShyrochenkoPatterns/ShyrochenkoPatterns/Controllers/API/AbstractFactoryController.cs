using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Helpers.Attributes;
using ShyrochenkoPatterns.Models.RequestModels.Posts;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using ShyrochenkoPatterns.ResourceLibrary;
using ShyrochenkoPatterns.Services.Interfaces.Proxy;
using ShyrochenkoPatterns.Services.Services.AbstractFactory.Client;
using ShyrochenkoPatterns.Services.Services.AbstractFactory.Post;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Controllers.API
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Validate]
    public class AbstractFactoryController : _BaseApiController
    {
        private Writer _poem;
        //private Writer _story;
        private Writer _proverb;
        private IProxy _proxy;

        public AbstractFactoryController(IStringLocalizer<ErrorsResource> localizer, IUnitOfWork unitOfWork, IMapper mapper, IProxy proxy)
            : base(localizer)
        {
            _poem = new Writer(new PoemFactory(unitOfWork, mapper));
            //_story = new Writer(new StoryFactory(unitOfWork, mapper));
            _proverb = new Writer(new ProverbFactory(unitOfWork, mapper));
            _proxy = proxy;
        }

        [HttpPost("Poem")]
        public async Task<IActionResult> CreatePoem([FromBody]PostRequestModel model)
        {
            var response = await _poem.Create(model);

            return Json(new JsonResponse<PostResponseModel>(response));
        }

        [HttpPost("Story")]
        public async Task<IActionResult> CreateStory([FromBody]PostRequestModel model)
        {
            //var response = await _story.Create(model);
            var response = await _proxy.Create(model);

            return Json(new JsonResponse<PostResponseModel>(response));
        }

        [HttpPost("Proverb")]
        public async Task<IActionResult> CreateProverb([FromBody]PostRequestModel model)
        {
            var response = await _proverb.Create(model);

            return Json(new JsonResponse<PostResponseModel>(response));
        }
    }
}
