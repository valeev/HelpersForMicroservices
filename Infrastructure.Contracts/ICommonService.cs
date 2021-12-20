namespace Infrastructure.Contracts;

/// <summary>
/// Common interface for all services
/// </summary>
public interface ICommonService
{
    /// <summary>
    /// Return current service info
    /// </summary>
    /// <returns></returns>
    Task<GetServiceInfoResponse> GetServiceInfo();
}
