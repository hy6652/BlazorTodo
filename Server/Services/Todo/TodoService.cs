using Microsoft.Azure.Cosmos;
using BlazorTodo.Shared;
using BlazorTodo.Server.Services.Cosmos;

namespace BlazorTodo.Server.Services.Todo
{
    public class TodoService
    {
        private readonly CosmosDbService _cosmosDbService;
        private readonly Container _container;

        public TodoService(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
            _container = cosmosDbService.GetContainer();
        }

        // Create
        public async Task CreateTodoAsync(TodoItem todo)
        {
            todo.Id = Guid.NewGuid().ToString();
            todo.Pk = todo.Id;

            if (todo.Title == null)
            {
                throw new ArgumentNullException("Title Needed");
            }

            await _container.AddModel(todo);
        }

        // Read
        public async Task<List<TodoItem>> GetAllTodoAsync()
        {
            return await _container.GetModelLinqQueryable<TodoItem>().GetListFromFeedIteratorAsync();
        }

        // Update
        public async Task UpdateTodoAsync(TodoItem todo)
        {
            await _container.EditModel(todo);
        }

        // Delete
        public async Task DeleteTodoItemAsync(TodoItem todo)
        {
            await _container.RemoveModel<TodoItem>(todo.Id, todo.Pk);
        }

        public async Task<List<TodoItem>> GetDateFilteredTodo(DateTime startDate, DateTime endDate, bool isDone)
        {
            return await _container.FilterTodo<TodoItem>(startDate, endDate, isDone).GetListFromFeedIteratorAsync();
        }

        public async Task<List<TodoItem>> GetTodoFilteredByTitle(string title)
        {
            return await _container.FilterTodoByTitle<TodoItem>(title).GetListFromFeedIteratorAsync();
        }
    }
}
