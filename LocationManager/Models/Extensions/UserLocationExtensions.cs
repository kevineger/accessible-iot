public static class UserLocationExtensions {
    public static UserLocationDTO ToUserLocationDTO(this UserLocation u) {
        return new UserLocationDTO(u.UserId) {
            Type = u.Type.ToString("g"), // Enum as string
            Location = u.Location.ToGPSLocationDTO()
        };
    }
}