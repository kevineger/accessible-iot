public static class UserLocationExtensions
{
    public static UserLocationDTO ToUserLocationDTO(this UserLocation u)
    {
        return new UserLocationDTO(u.Type.ToString("g"), u.UserId)
        {
            UserId = u.UserId.ToString(),
            Type = u.Type.ToString("g"),
            GPSLat = u.Location.gpsLat,
            GPSLong = u.Location.gpsLong
        };
    }
}