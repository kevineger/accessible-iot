using System;

public static class UserLocationExtensions
{
    public static UserLocationDTO ToUserLocationDTO(this UserLocation u)
    {
        return new UserLocationDTO(u.Type.ToString("g"), u.UserId)
        {
            UserId = u.UserId,
            Type = u.Type.ToString("g"),
            GPSLat = u.Location.gpsLat,
            GPSLong = u.Location.gpsLong
        };
    }

    public static UserLocation ToUserLocation(this UserLocationDTO userLocationDTO)
    {
        return new UserLocation()
        {
            Type = Enum.Parse<UserType>(userLocationDTO.Type),
            UserId = userLocationDTO.UserId,
            Location = new GPSLocation()
            {
                gpsLat = userLocationDTO.GPSLat,
                gpsLong = userLocationDTO.GPSLong
            }
        };
    }
}