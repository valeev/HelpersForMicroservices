namespace Infrastructure.Models;

/// <summary>
/// Base properties for models
/// </summary>
public abstract class BaseGuidProperty
{
    #region Properties

    /// <summary>
    /// Unique id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Record creation date
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Last update by
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string UpdatedBy { get; set; }

    /// <summary>
    /// Last update date
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    #endregion
}