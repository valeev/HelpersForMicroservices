using System.Net;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Api
{
    /// <summary>
    /// Global exception filter
    /// </summary>
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        #region Variables

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// On exception
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            var serviceError = new ServiceError();
            if (context.Exception is ServiceException exception)
            {
                serviceError.Code = exception.Code;
                serviceError.Message = exception.Message;
                if (exception.StatusCode.HasValue)
                {
                    serviceError.StatusCode = exception.StatusCode.Value;
                }
                else if (string.IsNullOrEmpty(exception.Code))
                {
                    serviceError.StatusCode = 200;
                }
                else
                {
                    serviceError.StatusCode = exception.Code.ToLower() switch
                    {
                        "notfound" => 404,
                        "notauthorized" => 401,
                        "invalidparameters" => 422,
                        _ => 500
                    };
                }

            }
            else
            {
                serviceError.StatusCode = (int)HttpStatusCode.InternalServerError;
                serviceError.Code = "notHandled";
                serviceError.Message = context.Exception.Message;
            }
            context.Result = new InternalServerErrorObjectResult(serviceError);
            context.HttpContext.Response.StatusCode = serviceError.StatusCode;
            context.ExceptionHandled = true;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Internal server error
        /// </summary>
        public class InternalServerErrorObjectResult : ObjectResult
        {
            public InternalServerErrorObjectResult(ServiceError error)
                : base(error) => StatusCode = error.StatusCode;
        }

        #endregion
    }
}
