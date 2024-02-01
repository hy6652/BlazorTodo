using Microsoft.AspNetCore.Mvc;
using BlazorTodo.Server.Services;

namespace BlazorTodo.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private TestTaskService _testTaskService;

        public TestController(TestTaskService testTaskService)
        {
            _testTaskService = testTaskService;
        }

        [HttpGet]
        public async Task<ActionResult> TestTaskResult()
        {
            await _testTaskService.SumPageSizeAsync();

            return Ok();
        }
    }
}
