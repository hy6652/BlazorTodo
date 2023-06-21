using Microsoft.Azure.Cosmos;
using BlazorTodo.Shared;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BlazorTodo.Server.Services
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
            //todo.Id = Guid.NewGuid().ToString();
            //todo.Pk = todo.Id;

            await _container.AddModel<TodoItem>(todo);
        }

        // Read
        public async Task<List<TodoItem>> GetAllTodoAsync()
        {
            return await _container.GetModelLinqQueryable<TodoItem>().GetListFromFeedIteratorAsync();
        }

        // Update
        public async Task UpdateTodoAsync(TodoItem todo)
        {
            await _container.EditModel<TodoItem>(todo);
        }

        // Delete
        public async Task DeleteTodoItemAsync(TodoItem todo)
        {
            await _container.RemoveModel<TodoItem>(todo.Id, todo.Pk);
        }
    }
}
