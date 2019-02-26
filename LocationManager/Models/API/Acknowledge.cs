using Newtonsoft.Json;

public class Acknowledge
{
    [JsonProperty("careTakerUserId")]
    public string CareTakerUserId { get; set; }

    [JsonProperty("careRecipientId")]
    public string CareRecipientId { get; set; }

    [JsonProperty("action")]
    public CareTakerAction Action { get; set; }

    [JsonProperty("careTakerLocation")]
    public GPSLocation CareTakerLocation { get; set; }

    [JsonProperty("careRecipientLocation")]
    public GPSLocation CareRecipientLocation { get; set; }

    public enum CareTakerAction
    {
        ProvideHelp
    }
}