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
        private BlobImageService _blobService;

        public TodoController(TodoService todoService, BlobImageService blobService)
        {
            _todoService = todoService;
            _blobService = blobService;
        }

        // cosmos
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

        [HttpPost]
        public async Task<ActionResult<TodoItem>> CreateTodoWithImage([FromForm] FileDto dto)
        {
            var file = dto.file;
            var todo = dto.todo;

            Stream stream = file.OpenReadStream();
            string fileName = file.FileName;

            string urlString = await _blobService.UploadImage(stream, fileName);
            todo.FileUrl = urlString;
            todo.FileName = fileName;

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
            string fileName = todo.FileName;
            await _blobService.DeleteImage(fileName);
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
