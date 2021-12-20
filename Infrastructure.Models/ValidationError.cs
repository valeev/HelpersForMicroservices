namespace Infrastructure.Models;

/// <summary>
/// Validation error
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Field name
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Error description
    /// </summary>
    public string Description { get; set; }
}