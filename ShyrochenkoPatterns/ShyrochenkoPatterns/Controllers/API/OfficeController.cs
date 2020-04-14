using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ShyrochenkoPatterns.Common.Exceptions;
using ShyrochenkoPatterns.Models.Facade;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.ResourceLibrary;
using ShyrochenkoPatterns.Services.Interfaces.Facade;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Controllers.API
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class OfficeController : _BaseApiController
    {
        private IMFU _mfu;
        public OfficeController(IStringLocalizer<ErrorsResource> errorsLocalizer, IMFU mfu)
            : base(errorsLocalizer)
        {
            _mfu = mfu;
        }

        [HttpPost("Copy")]
        public async Task<IActionResult> Copy([FromQuery]string text, [FromQuery]int count)
        {
            if (text == null)
                throw new CustomException(HttpStatusCode.BadRequest, "mfu", "Jammed paper!");

            var response = await _mfu.Copy(new Paper(text), count);

            return Json(new JsonResponse<List<Paper>>(response));
        }

        [HttpPost("Scan")]
        public async Task<IActionResult> Scan([FromQuery]string text)
        {
            if (text == null)
                throw new CustomException(HttpStatusCode.BadRequest, "mfu", "Yellow paint over!");

            var response = await _mfu.Scan(new Paper(text));

            return Json(new JsonResponse<byte[]>(response));
        }

        [HttpPost("Print")]
        public async Task<IActionResult> Print([FromQuery]byte[] array)
        {
            if (array == null)
                throw new CustomException(HttpStatusCode.BadRequest, "mfu", "Load paper!");

            var response = await _mfu.Print(array);

            return Json(new JsonResponse<Paper>(response));
        }
    }
}