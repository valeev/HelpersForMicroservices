namespace Infrastructure.Models;

/// <summary>
/// Service validation error
/// </summary>
public class ServiceValidationError : ServiceError
{
    /// <summary>
    /// Validation errors
    /// </summary>
    public List<ValidationError> ValidationErrors { get; set; }
}