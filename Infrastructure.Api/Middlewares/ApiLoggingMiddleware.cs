namespace Infrastructure.Api;

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

    public async Task Invoke(HttpContext httpContext, ILogger<ApiLoggingMiddleware> logger, IOptions<LogsConfig> logsConfig)
    {
        try
        {
            _logger = logger;

            var request = httpContext.Request;
            if (request.Path.StartsWithSegments(new PathString("/api")) &&
                (logsConfig.Value?.IgnoredPaths == null ||
                 !logsConfig.Value.IgnoredPaths.Any(p => p.Method.Equals(request.Method, StringComparison.InvariantCultureIgnoreCase)
                                                            && request.Path.StartsWithSegments(new PathString(p.Path)))))
            {
                var stopWatch = Stopwatch.StartNew();
                var requestBodyContent = await ReadRequestBody(request);
                var originalBodyStream = httpContext.Response.Body;
                using var responseBody = new MemoryStream();
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
                    responseBodyContent,
                    logsConfig);
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

    private static async Task<string> ReadRequestBody(HttpRequest request)
    {
        request.EnableBuffering();

        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer);
        var bodyAsText = Encoding.UTF8.GetString(buffer);
        request.Body.Seek(0, SeekOrigin.Begin);

        return bodyAsText;
    }

    private static async Task<string> ReadResponseBody(HttpResponse response)
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
                        string responseBody,
                        IOptions<LogsConfig> logsConfig)
    {
        if (logsConfig.Value.LogsMaximumLength > 0)
        {
            if (requestBody.Length > logsConfig.Value.LogsMaximumLength)
            {
                requestBody = $"(Truncated to {logsConfig.Value.LogsMaximumLength} chars) {requestBody[..logsConfig.Value.LogsMaximumLength]}";
            }

            if (responseBody.Length > logsConfig.Value.LogsMaximumLength)
            {
                responseBody = $"(Truncated to {logsConfig.Value.LogsMaximumLength} chars) {responseBody[..logsConfig.Value.LogsMaximumLength]}";
            }

            if (queryString.Length > logsConfig.Value.LogsMaximumLength)
            {
                queryString = $"(Truncated to {logsConfig.Value.LogsMaximumLength} chars) {queryString[..logsConfig.Value.LogsMaximumLength]}";
            }
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
