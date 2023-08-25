using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Azure.Storage.Sas;
using System.Diagnostics;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using BlazorTodo.Shared;

namespace BlazorTodo.Server.Services
{
    public class BlobTestServiceOptions
    {
        public string? ConnectionStringForTest { get; set; }
        public string? ContainerNameForTest { get; set; }
    }

    public class BlobTestService
    {
        public readonly BlobServiceClient client;
        public readonly string connectionStrinigForTest;
        public readonly string containerNameForTest;

        private readonly string blobName;
        private readonly string filePathToDownloadCsv;

        public BlobTestService(IOptions<BlobTestServiceOptions> options)
        {
            connectionStrinigForTest = options.Value.ConnectionStringForTest;
            containerNameForTest = options.Value.ContainerNameForTest;

            client = new BlobServiceClient(connectionStrinigForTest);

            blobName = "document.xlsx";
            filePathToDownloadCsv = @"C:\\Users\\고현영\\Downloads\\";
        }

        public Uri GetContainerSASUri(string containerName, string storedPolicyName = null)
        {
            var container = client.GetBlobContainerClient(containerName);

            // Check whether this BlobContainerClient object has been authorized with Shared Key.
            if (container.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for sone hour.
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

        public async Task<List<SearchableBlock>> GetExcelFromBlob()
        {
            Uri containerSasUri = GetContainerSASUri(containerNameForTest);
            BlobContainerClient blobContainerClient = new BlobContainerClient(containerSasUri);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            Stream stream = await blobClient.OpenReadAsync();
            List<SearchableBlock> blocks = await ReadExcelFromStream(stream);
            return blocks;
        }

        public async Task DownloadExcel()
        {
            BlobContainerClient blobContainerClient = client.GetBlobContainerClient(containerNameForTest);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
            await blobClient.DownloadToAsync(filePathToDownloadCsv + blobName);
        }

        public async Task<List<SearchableBlock>> ReadExcelFromStream(Stream stream)
        {
            var result = new List<SearchableBlock>();

            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            stream.Close();
            memoryStream.Position = 0;

            var excel = new XSSFWorkbook(memoryStream);
            int entireSheetNums = excel.NumberOfSheets;

            for (int i = 0; i < entireSheetNums; i++)
            {
                ISheet sheet = excel.GetSheetAt(i);
                IRow header = sheet.GetRow(0);
                int columnNums = header.LastCellNum;
                string sheetName = sheet.SheetName;

                int FirstSheetNum = sheet.FirstRowNum + 1;
                for (int j = FirstSheetNum; j <= sheet.LastRowNum; j++)
                {
                    var values = new List<string>();
                    IRow row = sheet.GetRow(j);

                    if (row == null || row.Cells.Count == 0)
                    {
                        continue;
                    }

                    for (int k = 0; k < columnNums; k++)
                    {
                        values.Add(row.GetCell(k)?.ToString() ?? "");
                    }

                    if (values.Count > 0)
                    {
                        var block = new SearchableBlock
                        {
                            Id = Guid.NewGuid().ToString(),
                            SheetName = sheetName,
                            Name = values[0],
                            Type1 = values[1],
                            Type2 = values[2],
                            FirstEmenrgence = values[3],
                            Location = values[4].Split(",").ToList(),
                            IsCaptured = values[5] 
                        };
                        result.Add(block);
                    }
                }
            }
            return result;
        }
    }
}
