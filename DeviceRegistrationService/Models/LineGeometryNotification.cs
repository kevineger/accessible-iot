public class LineGeometryNotification {
    public NotificationType Type => NotificationType.ShowDirections;
    public string ResourceUrl { get; set; }
    public LineGeometry LineGeometry { get; set; }
}