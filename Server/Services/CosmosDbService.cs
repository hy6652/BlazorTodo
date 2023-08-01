using BlazorTodo.Shared;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace BlazorTodo.Server.Services
{
    public class CosmosDbServiceOptions
    {
        public string? Account { get; set; }
        public string? Key { get; set; }
        public string? DatabaseName { get; set; }
        public string? ContainerName { get; set; }
    }

    public class CosmosDbService
    {
        private readonly CosmosClient client;
        private readonly string databaseName;
        private readonly string containerName;

        public CosmosDbService(IOptions<CosmosDbServiceOptions> options)
        {
            client = new CosmosClient(options.Value.Account, options.Value.Key);
            databaseName = options.Value.DatabaseName;
            containerName = options.Value.ContainerName;
        }

        public Container GetContainer()
        {
            var container = client.GetContainer(databaseName, containerName);
            return container;
        }
    }

    public static class CosmosDbServiceExtensions
    {
        public static async Task<T> GetModel<T>(this Container container, string id, string pk) where T : CosmosModelBase
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Parameter cannot be null", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(pk))
            {
                throw new ArgumentException("Parameter cannot be null", nameof(pk));
            }

            try
            {
                var response = await container.ReadItemAsync<T>(id, new PartitionKey(pk));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public static async Task AddModel<T>(this Container container, T model) where T : CosmosModelBase
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                throw new ArgumentException("Parameter cannot be null", nameof(model.Id));
            }

            if (string.IsNullOrWhiteSpace(model.Pk))
            {
                throw new ArgumentException("Parameter cannot be null", nameof(model.Pk));
            }

            try
            {
                await container.CreateItemAsync(model, new PartitionKey(model.Pk));
            }
            catch (CosmosException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task EditModel<T>(this Container container, T model) where T : CosmosModelBase
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                throw new ArgumentException("Parameter cannot be null", nameof(model.Id));
            }

            if (string.IsNullOrWhiteSpace(model.Pk))
            {
                throw new ArgumentException("Parameter cannot be null", nameof(model.Pk));
            }

            try
            {
               await container.UpsertItemAsync(model, new PartitionKey(model.Pk));
            }
            catch (CosmosException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task RemoveModel<T>(this Container container, string id, string pk) where T : CosmosModelBase
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Parameter cannot be null", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(pk))
            {
                throw new ArgumentException("Parameter cannot be null", nameof(pk));
            }

            try
            {
                await container.DeleteItemAsync<T>(id, new PartitionKey(pk));
            }
            catch (CosmosException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static IQueryable<T> OfCosmosItemType<T>(this IQueryable<T> query) where T : CosmosModelBase
        {
            var typeName = query.ElementType.Name;
            return query.Where(x => x.ClassType == "Todo");
        }

        public static IQueryable<T> GetModelLinqQueryable<T>(this Container container) where T : CosmosModelBase
        {
            return container.GetItemLinqQueryable<T>()
                .OfCosmosItemType()
                .AsQueryable();
        }

        public static async Task<List<T>> GetListFromFeedIteratorAsync<T>(this IQueryable<T> query) 
        {
            var result = new List<T>();
            using (var iterator = query.ToFeedIterator())
            {
                while (iterator.HasMoreResults)
                {
                    foreach (var item in await iterator.ReadNextAsync())
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        public static IQueryable<T> FilterTodo<T>(this Container container, DateTime startDate, DateTime endDate, bool isDone) where T : TodoItem
        {
            DateTime endDatePlusOne = endDate.AddDays(1);

            return container.GetItemLinqQueryable<T>()
                .Where(x => x.CreatedTime >= startDate && x.CreatedTime <= endDatePlusOne && x.IsDone == isDone)
                .AsQueryable();
        }

        public static IQueryable<T> FilterTodoByTitle<T>(this Container container, string title) where T : TodoItem
        {
            return container.GetItemLinqQueryable<T>()
                .Where(x => x.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .AsQueryable();
        }

        // csv upload with CommonModel
        public static async Task<List<T>> UploadCsvWithCommonModel<T> (this Container container, List<T> model) where T : CsvItem
        {
            List<T> result = new List<T>();
            foreach (var item in model)
            {
                var response = await container.CreateItemAsync(item, new PartitionKey(item.Pk));
                result.Add(response.Resource);
            }
            return result;
        }

        // return all csv
        public static IQueryable<T> GetCsvByClassType<T> (this Container container) where T : CsvItem
        {
            return container.GetItemLinqQueryable<T>()
                .Where(x => x.ClassType == "CsvItem")
                .AsQueryable();
        }

        public static IQueryable<T> GetCsvByTitle<T>(this Container container, string  csvTitle) where T : CsvItem
        {
            return container.GetCsvByClassType<T>()
                .Where(x => x.Title == csvTitle);
        }
    }
}
