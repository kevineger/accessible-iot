public static class GPSLocationExtensions
{
    public static GPSLocationDTO ToGPSLocationDTO(this GPSLocation g)
    {
        return new GPSLocationDTO
        {
            gpsLat = g.gpsLat,
            gpsLong = g.gpsLong
        };
    }
}