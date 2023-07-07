using Azure.Storage.Blobs;
using BlazorTodo.Shared;
using Microsoft.Extensions.Options;
using Azure.Storage.Sas;
using System.Diagnostics;
using Microsoft.Azure.Cosmos;

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

        private readonly CosmosDbService _cosmosDbService;
        private readonly Container _container;

        private readonly string containerNameForCsv;
        private readonly string filePathToDownloadCsv;

        private readonly CsvService _csvService;

        public BlobImageService(IOptions<BlobImageServiceOptions> options, CosmosDbService cosmosDbService, CsvService csvService)
        {
            connectionString = options.Value.ConnectionString;
            containerName = options.Value.ContainerName;
            client = new BlobServiceClient(connectionString);
            blobContainer = new BlobContainerClient(connectionString, containerName);

            _cosmosDbService = cosmosDbService;
            _container = cosmosDbService.GetContainer();

            containerNameForCsv = "csvcontainer";
            filePathToDownloadCsv = @"C:\\Users\\고현영\\Downloads\\";

            _csvService = csvService;
        }

        public async Task<string> UploadImage(Stream stream, string fileName)
        {
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

        // Upload csv to blob storage
        public async Task<List<CsvItem>> UploadCsv(IFormFile file, List<CsvModel> data)
        {
            // blob upload
            var blobContainerForCsv = new BlobContainerClient(connectionString, "csvcontainer");
            var createResponse = await blobContainerForCsv.CreateIfNotExistsAsync();

            if (createResponse != null && createResponse.GetRawResponse().Status == 200)
            {
                await blobContainerForCsv.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            }

            Stream stream = file.OpenReadStream();
            string fileName = Guid.NewGuid().ToString() + ".csv";

            var blob = blobContainerForCsv.GetBlobClient(fileName);
            await blob.UploadAsync(stream);

            // blob url
            var urlString = blob.Uri.ToString();

            // CommonModel
            List<CsvItem> csvItems = new List<CsvItem>();

            foreach (var csv in data)
            {
                CsvItem newItem = new CsvItem();
                newItem.Id = Guid.NewGuid().ToString();
                newItem.Pk = newItem.Id;
                newItem.BlobUrl = urlString;
                newItem.Title = fileName;

                newItem.FieldDict = new()
                {
                    {
                        "Id",
                        new()
                        {
                            IntValues = new() { csv.Id }
                        }
                    },
                    {
                        "Movie",
                        new()
                        {
                            StringValues = new() { csv.Movie }
                        }
                    },
                    {
                        "Rating",
                        new()
                        {
                            DoubleValues = new() { csv.Rating }
                        }
                    }
                };

                csvItems.Add(newItem);
            }

            var result = await _container.UploadCsvWithCommonModel<CsvItem>(csvItems);
            return result;
        }


        public async Task<List<BlobTitleModel>> GetAllCsvTitle()
        {
            var results = await _container.GetCsvByClassType<CsvItem>().GetListFromFeedIteratorAsync();
            var distinctTitles = results.Select(x => x.Title).Distinct();

            List<BlobTitleModel> titles = new List<BlobTitleModel>();
            foreach (var item in distinctTitles)
            {
                BlobTitleModel newModel = new BlobTitleModel();
                newModel.Title = item;
                titles.Add(newModel);
            }
            return titles;
        }
        
        // Download all csv from blob storage
        public async Task DownloadCsv()
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(containerNameForCsv);
            foreach (var blob in containerClient.GetBlobs())
            {
                string blobName = blob.Name;
                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.DownloadToAsync(filePathToDownloadCsv + blobName);
            }
        }

        public async Task DownloadOneCsvByTitle(BlobTitleModel blobTitleModel)
        {
            string csvTitle = blobTitleModel.Title;
            var blobClient = client.GetBlobContainerClient(containerNameForCsv).GetBlobClient(csvTitle);
            await blobClient.DownloadToAsync(filePathToDownloadCsv + csvTitle);
        }

        public async Task DownloadOneCsvFromCosmos(BlobTitleModel blobTitleModel)
        {
            string csvTitle = blobTitleModel.Title;
            IEnumerable<CsvItem> csvList = await _container.GetCsvByTitle<CsvItem>(csvTitle).GetListFromFeedIteratorAsync();

            var blobUrl = csvList.Select(x => x.BlobUrl).First().ToString();
            BlobClient blobClient = new BlobClient(new Uri(blobUrl));
            await blobClient.DownloadToAsync(filePathToDownloadCsv + csvTitle);
        }

        // get csv from blob storage, read the file with csvhelper to display
        public async Task<List<CsvModel>> GetCsvFromBlobToReadCsv(BlobTitleModel blobTitleModel)
        {
            Uri containerSasUri = GetContainerSASUri(containerNameForCsv);
            BlobContainerClient cotainerClient = new BlobContainerClient(containerSasUri);
            BlobClient blobClient = cotainerClient.GetBlobClient(blobTitleModel.Title);

            Stream stream = await blobClient.OpenReadAsync();
            List<CsvModel> data = await _csvService.ReadSelectedCsv(stream);

            return data;
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

        // Get Container SAS
        public Uri GetContainerSASUri(string containerName, string storedPolicyName = null)
        {
            var container = client.GetBlobContainerClient(containerName);

            // Check whether this BlobContainerClient object has been authorized with Shared Key.
            if (container.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for one hour.
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = container.Name,
                    Resource = "c"
                };

                if (storedPolicyName == null)
                {
                    sasBuilder.StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1);
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                    sasBuilder.SetPermissions(BlobContainerSasPermissions.All);
                }
                else
                {
                    sasBuilder.Identifier = storedPolicyName;
                }

                Uri sasUri = container.GenerateSasUri(sasBuilder);

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
