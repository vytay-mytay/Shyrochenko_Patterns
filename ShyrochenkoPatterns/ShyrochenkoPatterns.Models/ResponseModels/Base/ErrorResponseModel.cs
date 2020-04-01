using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using ShyrochenkoPatterns.Common.Constants;
using ShyrochenkoPatterns.Common.Exceptions;
using ShyrochenkoPatterns.Common.Extensions;
using ShyrochenkoPatterns.ResourceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ShyrochenkoPatterns.Models.ResponseModels
{
    public class ErrorResponseModel
    {
        private readonly IStringLocalizer<ErrorsResource> _errorsLocalizer;

        public ErrorResponseModel(IStringLocalizer<ErrorsResource> errorsLocalizer)
        {
            Errors = Errors.Empty();
            _errorsLocalizer = errorsLocalizer;
        }

        [JsonRequired]
        [JsonProperty("_v")]
        public string Version { get; set; } = "1.0";

        [JsonRequired]
        [JsonProperty("code")]
        public string Code { get; set; }

        //[JsonProperty("message")]
        //public string Message { get; set; }

        [JsonProperty("stacktrace")]
        public string StackTrace { get; set; }

        [JsonProperty("errors")]
        public List<ErrorKeyValue> Errors { get; set; }

        public void AddError(string key, string message)
        {
            var keyFromErrors = Errors.FirstOrDefault(x => x.Key.ToLower() == key.ToLower());

            if (keyFromErrors != null)
            {
                keyFromErrors.Message = message;
            }
            else
            {
                Errors.Add(new ErrorKeyValue(key, message));
            }
        }

        public void BuildErrors(ModelStateDictionary modelState)
        {
            Errors.AddRange(modelState.Where(x => x.Value.Errors.Count > 0).Select(x => new ErrorKeyValue(x.Key.Replace("model.", String.Empty), x.Value.Errors.Last().ErrorMessage)).ToList());
        }

        public void BuildLocalizedErrors(ModelStateDictionary modelState)
        {
            Errors.AddRange(modelState.Where(x => x.Value.Errors.Count > 0).Select(x => new ErrorKeyValue(x.Key.Replace("model.", String.Empty), _errorsLocalizer[x.Value.Errors.Last().ErrorMessage])).ToList());
        }

        #region BadRequest

        public ContentResult BadRequest()
        {
            return Error(HttpStatusCode.BadRequest);
        }

        public ContentResult BadRequest(string key, string message)
        {
            AddError(key, message);
            return BadRequest();
        }

        public ContentResult BadRequest(string key, int code)
        {
            AddError(key, _errorsLocalizer[code.ToString()]);
            return BadRequest();
        }

        public ContentResult LocalizedBadRequest(string key, string code)
        {
            AddError(key, _errorsLocalizer[code]);
            return BadRequest();
        }

        #endregion

        #region InternalServerError

        public ContentResult InternalServerError(string stacktrace = null)
        {
            StackTrace = stacktrace;
            return Error(HttpStatusCode.InternalServerError);
        }

        public ContentResult InternalServerError(string key, string message, string stacktrace = null)
        {
            AddError(key, message);
            return InternalServerError(stacktrace);
        }

        public ContentResult InternalServerError(string key, int code)
        {
            string error = _errorsLocalizer[code.ToString()];
            AddError(key.ToString(), error);
            return InternalServerError();
        }

        #endregion

        #region NotFound

        public ContentResult NotFound()
        {
            return Error(HttpStatusCode.NotFound);
        }

        public ContentResult NotFound(string key, string message)
        {
            AddError(key, message);
            return NotFound();
        }

        public ContentResult NotFound(string key, int code)
        {
            string error = _errorsLocalizer[code.ToString()];
            AddError(key.ToString(), error);
            return NotFound();
        }

        #endregion

        public ContentResult Error(CustomException ex)
        {
            AddError(ex.Key, ex.Message);
            return Error(ex.Code);
        }

        public ContentResult Error(HttpStatusCode code)
        {
            var codeDescription = "";

            switch (code)
            {
                case HttpStatusCode.BadRequest:
                    codeDescription = ErrorCode.BadRequest;
                    break;
                case HttpStatusCode.Unauthorized:
                    codeDescription = ErrorCode.Unauthorized;
                    break;
                case HttpStatusCode.Forbidden:
                    codeDescription = ErrorCode.Forbidden;
                    break;
                case HttpStatusCode.NotFound:
                    codeDescription = ErrorCode.NotFound;
                    break;
                case HttpStatusCode.MethodNotAllowed:
                    codeDescription = ErrorCode.NotAllowed;
                    break;
                case HttpStatusCode.NotAcceptable:
                    codeDescription = ErrorCode.NotAcceptable;
                    break;
                case HttpStatusCode.RequestTimeout:
                    codeDescription = ErrorCode.RequestTimeout;
                    break;
                case HttpStatusCode.Conflict:
                    codeDescription = ErrorCode.Conflict;
                    break;
                case HttpStatusCode.UnprocessableEntity:
                    codeDescription = ErrorCode.Unprocessable;
                    break;
                case HttpStatusCode.InternalServerError:
                    codeDescription = ErrorCode.InternalServerError;
                    break;
                case HttpStatusCode.GatewayTimeout:
                    codeDescription = ErrorCode.GatewayTimeout;
                    break;
            }

            Code = codeDescription;
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(this, new JsonSerializerSettings { Formatting = Formatting.Indented }),
                StatusCode = (int)code,
                ContentType = "application/json"
            };
        }
    }

    public class ErrorKeyValue
    {
        public ErrorKeyValue(string key, string msg)
        {
            Key = key;
            Message = msg;
        }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
