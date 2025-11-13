namespace Juga.Abstractions.Client;

/// <summary>
/// Backend(Api) ve Frontend(Mvc) tarafında token içindeki oturum bilgilerine erişim için kullanılacak
/// </summary>
public interface IUserContextProvider
{
    /// <summary>
    /// Client Id bilgisi
    /// </summary>
    string? ClientId { get; set; }

    List<string>? Projects { get; set; }
    string? IdentityNumber { get; set; }
    List<string>? Roles { get; set; }

    /// <summary>
    /// Client Name bilgisi
    /// </summary>
    string? ClientName { get; set; }

    string? Email { get; set; }

    /// <summary>
    /// Client Ip bilgisi
    /// </summary>
    string? ClientIp { get; set; }

    /// <summary>
    /// User Code bilgisi
    /// </summary>
    string? UserCode { get; set; }

    /// <summary>
    /// Kurumsal kullanıcı bilgisi
    /// </summary>
    string? CorporateUser { get; set; }

    /// <summary>
    /// Birlik Kodu
    /// </summary>
    string? TroopCode { get; set; }

    /// <summary>
    /// İl Kodu
    /// </summary>
    string? CityCode { get; set; }

    /// <summary>
    /// İlçe Kodu
    /// </summary>
    string? DistrictCode { get; set; }
}