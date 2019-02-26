public static class AcknowledgeExtensions
{
    public static AcknowledgeDTO ToAcknowledgeDTO(this Acknowledge a)
    {
        return new AcknowledgeDTO(a.CareRecipientId, a.Action.ToString("g"))
        {
            CareTakerId = a.CareTakerUserId,
            CareRecipientId = a.CareRecipientId,
            Action = a.Action.ToString("g"),
            CareTakerGPSLat = a.CareTakerLocation.gpsLat,
            CareTakerGPSLong = a.CareTakerLocation.gpsLong,
            CareRecipientGPSLat = a.CareRecipientLocation.gpsLat,
            CareRecipientGPSLong = a.CareRecipientLocation.gpsLong
        };
    }
}