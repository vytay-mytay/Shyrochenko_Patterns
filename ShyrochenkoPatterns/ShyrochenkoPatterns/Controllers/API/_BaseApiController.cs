using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using ShyrochenkoPatterns.Common.Constants;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.ResourceLibrary;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Controllers.API
{
    [ApiVersion("1.0")]
    public class _BaseApiController : Controller
    {
        protected ErrorResponseModel Errors;
        private IStringLocalizer<ErrorsResource> _errorsLocalizer;

        public _BaseApiController(IStringLocalizer<ErrorsResource> errorsLocalizer)
        {
            _errorsLocalizer = errorsLocalizer;
            Errors = new ErrorResponseModel(_errorsLocalizer);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Forbidden()
        {
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(new ErrorResponseModel(_errorsLocalizer)
                {
                    Code = ErrorCode.Forbidden,
                }, new JsonSerializerSettings { Formatting = Formatting.Indented }),
                StatusCode = 403,
                ContentType = "application/json"
            };
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> IsAdminAsync(ClaimsPrincipal User)
        {
            return User.IsInRole(Role.Admin);
        }
    }
}