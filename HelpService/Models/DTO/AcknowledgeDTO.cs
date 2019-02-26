using System;
using Microsoft.WindowsAzure.Storage.Table;

public class AcknowledgeDTO : TableEntity
{
    public AcknowledgeDTO(string careRecipientId, string action)
    {
        this.PartitionKey = careRecipientId;
        this.RowKey = $"{careRecipientId};{action}";
    }

    public AcknowledgeDTO() { }

    public string CareTakerId { get; set; }

    public string CareRecipientId { get; set; }

    public string Action { get; set; }

    public double CareTakerGPSLat { get; set; }

    public double CareTakerGPSLong { get; set; }

    public double CareRecipientGPSLat { get; set; }

    public double CareRecipientGPSLong { get; set; }
}