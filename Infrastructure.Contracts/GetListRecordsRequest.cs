namespace Infrastructure.Contracts;

/// <summary>
/// Abstract class with main filters for Get records methods
/// </summary>
public abstract class GetListRecordsRequest
{
    /// <summary>
    /// Search string
    /// </summary>
    public string Search { get; set; } = null!;

    /// <summary>
    /// Sort by field
    /// Possible values: fieldName_desk, fieldName_asc, fieldName
    /// </summary>
    public string SortBy { get; set; } = null!;

    /// <summary>
    /// Amount of records for taking
    /// </summary>
    public int Limit { get; set; } = 10;

    /// <summary>
    /// Amount of records for skipping
    /// </summary>
    public int Skip { get; set; }
}