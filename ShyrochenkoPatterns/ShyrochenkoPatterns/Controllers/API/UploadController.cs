using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ShyrochenkoPatterns.Common.Constants;
using ShyrochenkoPatterns.Models.Enums;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.ResourceLibrary;
using ShyrochenkoPatterns.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Controllers.API
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = Role.User*/)]
    public class UploadController : _BaseApiController
    {
        private IImageService _imageService;
        private IHttpContextAccessor _httpContextAccessor;
        private ILogger<UploadController> _logger;

        public UploadController(IStringLocalizer<ErrorsResource> localizer,
            IImageService imageService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UploadController> logger)
             : base(localizer)
        {
            _httpContextAccessor = httpContextAccessor;
            _imageService = imageService;
            _logger = logger;
        }

        // POST api/v1/upload/image
        /// <summary>
        /// Upload Image
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/upload/image
        ///     
        /// </remarks>
        /// <returns>HTTP 200, or errors with an HTTP 500</returns>
        [SwaggerResponse(200, ResponseMessages.RequestSuccessful, typeof(JsonResponse<ImageResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.InvalidData, typeof(ErrorResponseModel))]
        [SwaggerResponse(401, ResponseMessages.Unauthorized, typeof(ErrorResponseModel))]
        [SwaggerResponse(403, ResponseMessages.Forbidden, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [DisableRequestSizeLimit]
        [HttpPost("Image")]
        public async Task<IActionResult> Image(IFormFile file, ImageType imageType)
        {
            if (file == null)
                return Errors.BadRequest("Image", "Failed image uploading");

            var response = await _imageService.UploadOne(file, imageType);

            return Json(new JsonResponse<ImageResponseModel>(response));
        }

        // POST api/v1/upload/multipleimages
        /// <summary>
        /// Upload multiple Images
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/upload/multipleimages
        ///     
        /// </remarks>
        /// <returns>HTTP 200, or errors with an HTTP 500</returns>
        [SwaggerResponse(200, ResponseMessages.RequestSuccessful, typeof(JsonResponse<List<ImageResponseModel>>))]
        [SwaggerResponse(400, ResponseMessages.InvalidData, typeof(ErrorResponseModel))]
        [SwaggerResponse(401, ResponseMessages.Unauthorized, typeof(ErrorResponseModel))]
        [SwaggerResponse(403, ResponseMessages.Forbidden, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [DisableRequestSizeLimit]
        [HttpPost("MultipleImages")]
        public async Task<IActionResult> MultipleImages(List<IFormFile> images, ImageType imageType)
        {
            if (!images.Any())
                return Errors.BadRequest("Image", "Failed image uploading");

            var response = await _imageService.UploadMultiple(images, imageType);

            return Json(new JsonResponse<List<ImageResponseModel>>(response));
        }
    }
}