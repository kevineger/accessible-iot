using Microsoft.WindowsAzure.Storage.Table;

public class User : TableEntity
{
    public string ConnectionString { get; set; }
}