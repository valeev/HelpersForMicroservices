namespace Infrastructure.Models;

/// <summary>
/// Custom service exception
/// </summary>
public class ServiceException : Exception
{
    #region Properties

    /// <summary>
    /// HTTP Status code
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// Error code
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Validation errors
    /// </summary>
    public List<ValidationError> ValidationErrors { get; }

    #endregion

    #region Constructors

    protected ServiceException()
    {
    }

    public ServiceException(ServiceError serviceError)
        : this(serviceError.StatusCode, serviceError.Code, serviceError.Message)
    {

    }

    public ServiceException(ServiceValidationError serviceValidationError)
        : this(serviceValidationError.StatusCode, serviceValidationError.Code, serviceValidationError.Message, serviceValidationError.ValidationErrors)
    {

    }

    public ServiceException(string code)
    {
        Code = code;
    }

    public ServiceException(int statusCode, string code)
    {
        StatusCode = statusCode;
        Code = code;
    }

    public ServiceException(string message, params object[] args)
        : this(string.Empty, message, args)
    {
    }

    public ServiceException(string code, string message, params object[] args)
        : this(null, code, message, args)
    {
    }

    public ServiceException(int statusCode, string code, string message, params object[] args)
        : this(null, statusCode, code, message, null, args)
    {
    }

    public ServiceException(int statusCode, string code, string message, List<ValidationError> validationErrors, params object[] args)
        : this(null, statusCode, code, message, validationErrors, args)
    {

    }

    public ServiceException(Exception innerException, string message, params object[] args)
        : this(innerException, null, message, args)
    {
    }

    public ServiceException(Exception innerException, string code, string message, params object[] args)
        : base(string.Format(message, args), innerException)
    {
        Code = code;
    }

    public ServiceException(Exception innerException, int statusCode, string code, string message, params object[] args)
        : base(string.Format(message, args), innerException)
    {
        StatusCode = statusCode;
        Code = code;
    }

    public ServiceException(Exception innerException, int statusCode, string code, string message, List<ValidationError> validationErrors, params object[] args)
        : base(string.Format(message, args), innerException)
    {
        StatusCode = statusCode;
        Code = code;
        ValidationErrors = validationErrors;
    }

    #endregion
}