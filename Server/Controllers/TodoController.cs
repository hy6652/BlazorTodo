using BlazorTodo.Client.Pages;
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
    }
}
