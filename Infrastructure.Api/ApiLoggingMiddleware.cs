using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Logging
{
    /// <summary>
    /// Middleware for logging all requests and responses
    /// </summary>
    public class ApiLoggingMiddleware
    {
        #region Variables

        private readonly RequestDelegate _next;
        private ILogger<ApiLoggingMiddleware> _logger;

        #endregion

        #region Constructors

        public ApiLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        public async Task Invoke(HttpContext httpContext, ILogger<ApiLoggingMiddleware> logger)
        {
            try
            {
                _logger = logger;

                var request = httpContext.Request;
                if (request.Path.StartsWithSegments(new PathString("/api")))
                {
                    var stopWatch = Stopwatch.StartNew();
                    var requestBodyContent = await ReadRequestBody(request);
                    var originalBodyStream = httpContext.Response.Body;
                    using (var responseBody = new MemoryStream())
                    {
                        var response = httpContext.Response;
                        response.Body = responseBody;
                        await _next(httpContext);
                        stopWatch.Stop();

                        string responseBodyContent = null;
                        responseBodyContent = await ReadResponseBody(response);
                        await responseBody.CopyToAsync(originalBodyStream);

                        SafeLog(stopWatch.ElapsedMilliseconds,
                            response.StatusCode,
                            request.Method,
                            request.Path,
                            request.QueryString.ToString(),
                            requestBodyContent,
                            responseBodyContent);
                    }
                }
                else
                {
                    await _next(httpContext);
                }
            }
            catch (Exception)
            {
                await _next(httpContext);
            }
        }

        #region Helpers

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private void SafeLog(long elapsedMilliseconds,
                            int statusCode,
                            string method,
                            string path,
                            string queryString,
                            string requestBody,
                            string responseBody)
        {
            if (path.ToLower().StartsWith("/api/login"))
            {
                requestBody = "(Request logging disabled for /api/login)";
                responseBody = "(Response logging disabled for /api/login)";
            }

            if (requestBody.Length > 200)
            {
                requestBody = $"(Truncated to 200 chars) {requestBody.Substring(0, 200)}";
            }

            if (responseBody.Length > 200)
            {
                responseBody = $"(Truncated to 200 chars) {responseBody.Substring(0, 200)}";
            }

            if (queryString.Length > 200)
            {
                queryString = $"(Truncated to 200 chars) {queryString.Substring(0, 200)}";
            }

            _logger.LogInformation(JsonConvert.SerializeObject(new
            {
                ElapsedMilliseconds = elapsedMilliseconds,
                StatusCode = statusCode,
                Method = method,
                Path = path,
                QueryString = queryString,
                RequestBody = requestBody,
                ResponseBody = responseBody
            }));
        }

        #endregion
    }
}
