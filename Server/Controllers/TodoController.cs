using BlazorTodo.Server.Services;
using BlazorTodo.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTodo.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private TodoService _todoService;
        
        public TodoController(TodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoItem>>> GetAllTodo()
        {
            return await _todoService.GetAllTodoAsync();
        }


        [HttpPost]
        public async Task<ActionResult<TodoItem>> CreateTodo(TodoItem todo)
        {
            await _todoService.CreateTodoAsync(todo);
            return Ok();
        }

        [HttpPatch]
        public async Task<ActionResult<TodoItem>> UpdateTodo(TodoItem todo)
        {
            await _todoService.UpdateTodoAsync(todo);
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> DeleteTodo(TodoItem todo)
        {
            await _todoService.DeleteTodoItemAsync(todo);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoItem>>> GetFilteredTodo(DateTime startDate, DateTime endDate, bool isDone)
        {
            return await _todoService.GetDateFilteredTodo(startDate, endDate, isDone);
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoItem>>> GetTodoFilteredByTitle(string title)
        {
            return await _todoService.GetTodoFilteredByTitle(title);
        }
    }
}
