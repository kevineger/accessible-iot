using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

public static class BlobHelper {
    private static async Task<CloudBlockBlob> GetBlockBlobAsync(string storageAccString, string blobName) {
        var storageAccount = CloudStorageAccount.Parse(storageAccString);
        // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
        var cloudBlobClient = storageAccount.CreateCloudBlobClient();

        var cloudBlobContainer = cloudBlobClient.GetContainerReference("dankblobs");
        await cloudBlobContainer.CreateAsync();

        // Set the permissions so the blobs are public. 
        var permissions = new BlobContainerPermissions
        {
            PublicAccess = BlobContainerPublicAccessType.Blob
        };
        await cloudBlobContainer.SetPermissionsAsync(permissions);
        
        return cloudBlobContainer.GetBlockBlobReference(blobName);
    }
    public static async Task PostRouteToBlob(string storageAccString, string routeId, string blob)
    {
        var cloudBlockBlob = await GetBlockBlobAsync(storageAccString, routeId);
        await cloudBlockBlob.UploadTextAsync(blob);
    }

    public static async Task<string> GetRouteFromBlob(string storageAccString, string routeId)
    {
        var cloudBlockBlob = await GetBlockBlobAsync(storageAccString, routeId);
        return await cloudBlockBlob.DownloadTextAsync();
    }
}