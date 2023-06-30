using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using BlazorTodo.Shared;
using Microsoft.Extensions.Options;

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
    }
}