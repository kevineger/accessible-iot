using System;
using Microsoft.WindowsAzure.Storage.Table;

public class UserLocationDTO : TableEntity
{
    public UserLocationDTO(string type, string userId)
    {
        this.PartitionKey = type;
        this.RowKey = userId.ToString();
    }

    public UserLocationDTO() { }

    public string UserId { get; set; }

    public string Type { get; set; }

    public double GPSLat { get; set; }

    public double GPSLong { get; set; }
}