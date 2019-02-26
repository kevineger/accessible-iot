public class Acknowledge
{
    public string CareTakerUserId { get; set; }
    public string CareRecipientId { get; set; }
    public CareTakerAction Action { get; set; }
    public GPSLocation CareTakerLocation { get; set; }
    public GPSLocation CareRecipientLocation { get; set; }

    public enum CareTakerAction
    {
        ProvideHelp
    }
}