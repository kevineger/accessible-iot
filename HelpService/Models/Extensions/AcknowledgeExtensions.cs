public static class AcknowledgeExtensions
{
    public static AcknowledgeDTO ToAcknowledgeDTO(this Acknowledge a)
    {
        return new AcknowledgeDTO(a.CareRecipientId, a.Action)
        {
            CareTakerId = a.CareTakerUserId,
            CareRecipientId = a.CareRecipientId,
            Action = a.Action,
            CareTakerGPSLat = a.CareTakerLocation.gpsLat,
            CareTakerGPSLong = a.CareTakerLocation.gpsLong,
            CareRecipientGPSLat = a.CareRecipientLocation.gpsLat,
            CareRecipientGPSLong = a.CareRecipientLocation.gpsLong
        };
    }
}