using System;
using Microsoft.WindowsAzure.Storage.Table;

public class UserLocationDTO : TableEntity
{
    public UserLocationDTO(Guid userId)
    {
        this.PartitionKey = "ConstantPartitionKey";
        this.RowKey = userId.ToString();
    }

    public UserLocationDTO() { }

    public string Type { get; set; }

    public GPSLocationDTO Location { get; set; }
}