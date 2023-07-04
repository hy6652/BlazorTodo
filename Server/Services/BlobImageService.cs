using Azure.Storage.Blobs;
using BlazorTodo.Shared;
using Microsoft.Extensions.Options;
using Azure.Storage.Sas;
using System.Diagnostics;

namespace BlazorTodo.Server.Services
{
    public class FileDto
    {
        public IFormFile? file { get; set; }
        public TodoItem? todo { get; set; }
    }

    public class BlobImageServiceOptions
    {
        public string? ConnectionString { get; set; }
        public string? ContainerName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountKey { get; set; }
    }

    public class BlobImageService
    {
        private readonly BlobServiceClient client;
        private readonly string connectionString;
        private readonly string containerName;
        private readonly BlobContainerClient blobContainer;

        public BlobImageService(IOptions<BlobImageServiceOptions> options)
        {
            connectionString = options.Value.ConnectionString;
            containerName = options.Value.ContainerName;
            client = new BlobServiceClient(connectionString);
            blobContainer = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<string> UploadImage(Stream stream, string fileName)
        {
            //var container = new BlobContainerClient(connectionString, containerName);
            var createResponse = await blobContainer.CreateIfNotExistsAsync();

            if (createResponse != null && createResponse.GetRawResponse().Status == 200)
            {
                await blobContainer.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            }

            var blob = blobContainer.GetBlobClient(fileName);
            await blob.UploadAsync(stream, overwrite: true);
            var urlString = blob.Uri.ToString();
            return urlString;
        }

        public async Task DeleteImage(string fileName)
        {
            var blobClient = client.GetBlobContainerClient(containerName).GetBlobClient(fileName);
            await blobClient.DeleteAsync();
        }

        // 사용 안함
        public async Task<List<ImageUpload>> GetImages()
        {
            List<ImageUpload> result = new List<ImageUpload>();

            foreach (var blob in client.GetBlobContainerClient(containerName).GetBlobs())
            {
                var blobClient = client.GetBlobContainerClient(containerName).GetBlobClient(blob.Name);
                ImageUpload upload = new ImageUpload
                {
                    FileName = blobClient.Name,
                    FileStorageUrl = blobClient.Uri.ToString()
                };
                result.Add(upload);
            }

            return result;
        }

        // Blob Service SAS
        public async Task<Uri> CreateServiceSasForBlob(string blobName, string storedPolicyName = null)
        {
            var containerClient = client.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (blobClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = containerName,
                    Resource = "b",
                    BlobName = blobClient.Name
                };

                if (storedPolicyName == null)
                {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddDays(1);
                    sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);
                }
                else
                {
                    sasBuilder.Identifier = storedPolicyName;
                }

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
                return sasUri;
            }
            else
            {
                Console.WriteLine(@"BlobContainerClient must be authorized with Shared Key 
                          credentials to create a service SAS.");

                return new Uri("");
            }
        }
    }
}