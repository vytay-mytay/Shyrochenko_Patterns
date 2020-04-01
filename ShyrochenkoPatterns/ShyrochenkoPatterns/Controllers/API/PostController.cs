using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Models.RequestModels.Posts;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using ShyrochenkoPatterns.ResourceLibrary;
using ShyrochenkoPatterns.Services.Services.Client;
using ShyrochenkoPatterns.Services.Services.Post;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Controllers.API
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class PostController : _BaseApiController
    {
        //private IUnitOfWork _unitOfWork;
        //private IMapper _mapper;
        private Writer _poem;
        private Writer _story;
        private Writer _proverb;

        public PostController(IStringLocalizer<ErrorsResource> localizer, IUnitOfWork unitOfWork, IMapper mapper)
            : base(localizer)
        {
            //_unitOfWork = unitOfWork;
            //_mapper = mapper;
            _poem = new Writer(new PoemFactory(unitOfWork, mapper));
            _story = new Writer(new StoryFactory(unitOfWork, mapper));
            _proverb = new Writer(new ProverbFactory(unitOfWork, mapper));
        }

        [HttpPost("Poem")]
        public async Task<IActionResult> CreatePoem([FromBody]PostRequestModel model)
        {
            //var response = await new Writer(new PoemFactory(_unitOfWork, _mapper)).Create(model);
            var response = await _poem.Create(model);

            return Json(new JsonResponse<PostResponseModel>(response));
        }

        [HttpPost("Story")]
        public async Task<IActionResult> CreateStory([FromBody]PostRequestModel model)
        {
            //var response = await new Writer(new PoemFactory(_unitOfWork, _mapper)).Create(model);
            var response = await _story.Create(model);

            return Json(new JsonResponse<PostResponseModel>(response));
        }

        [HttpPost("Proverb")]
        public async Task<IActionResult> CreateProverb([FromBody]PostRequestModel model)
        {
            //var response = await new Writer(new PoemFactory(_unitOfWork, _mapper)).Create(model);
            var response = await _proverb.Create(model);

            return Json(new JsonResponse<PostResponseModel>(response));
        }
    }
}
