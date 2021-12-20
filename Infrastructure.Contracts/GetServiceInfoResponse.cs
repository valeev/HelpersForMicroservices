namespace Infrastructure.Contracts;

/// <summary>
/// Response to getting service info
/// </summary>
public class GetServiceInfoResponse
{
    #region Properties

    /// <summary>
    /// Service information properties
    /// </summary>
    public Dictionary<string, string> Information { get; set; } = null!;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="information">Information</param>
    public GetServiceInfoResponse(Dictionary<string, string> information)
    {
        Information = information;
    }

    #endregion
}