namespace Juga.Abstractions.Data.Enums;

/// <summary>
/// Veritabanından çekilen veride yapılan değişiklikleri entitry framework tarafından takip edilip edilmememsinin tipleri
/// Connected default AsTracking , Disconnected default AsNoTracking
/// </summary>
public enum TrackingBehaviour
{
    ContextDefault = 1,
    AsTracking = 2,
    AsNoTracking = 3
}