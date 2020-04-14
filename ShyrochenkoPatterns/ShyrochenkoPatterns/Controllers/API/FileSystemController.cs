using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ShyrochenkoPatterns.Models.Composite;
using ShyrochenkoPatterns.Models.Enums;
using ShyrochenkoPatterns.Models.RequestModels.Composite;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.Models.ResponseModels.Composite;
using ShyrochenkoPatterns.ResourceLibrary;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Controllers.API
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class FileSystemController : _BaseApiController
    {
        public FileSystemController(IStringLocalizer<ErrorsResource> errorsLocalizer) : base(errorsLocalizer)
        {

        }

        [HttpPost]
        public async Task<IActionResult> Add(ComponentRequestModel model)
        {
            var response = new ComponentResponseModel();
            switch (model.Type)
            {
                case ComponentType.Directory:
                    response = Composite.Component.Get(model.Id).Add(new Directory(model.Name));
                    break;
                case ComponentType.File:
                    response = Composite.Component.Get(model.Id).Add(new File(model.Name, model.Size.HasValue ? model.Size.Value : 1));
                    break;
            }

            return Json(new JsonResponse<ComponentResponseModel>(response));
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var response = Composite.Component.Get(id).Display();

            return Json(new JsonResponse<ComponentResponseModel>(response));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            Composite.Component.Remove(id);

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("Done.")));
        }
    }
}