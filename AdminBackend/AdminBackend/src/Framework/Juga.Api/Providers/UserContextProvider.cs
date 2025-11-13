namespace Juga.Api.Providers;

/// <summary>
///     Backend(Api) tarafında token içindeki oturum bilgilerine erişim için kullanılacak
/// </summary>
public class ClientInfoProvider : IUserContextProvider
{
    public string ClientId { get; set; }
    public List<string> Projects { get; set; }
    public string IdentityNumber { get; set; }
    public List<string> Roles { get; set; }
    public string ClientName { get; set; }
    public string Email { get; set; }
    public string ClientIp { get; set; }
    public string UserCode { get; set; }
    public string CorporateUser { get; set; }
    public string TroopCode { get; set; }
    public string CityCode { get; set; }
    public string DistrictCode { get; set; }
}