using Microsoft.AspNetCore.Mvc;
using BlazorTodo.Shared;
using BlazorTodo.Server.Services.Utility;

namespace BlazorTodo.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RegexController : ControllerBase
    {
        private RegexService _regexService;

        public RegexController(RegexService regexService)
        {
            _regexService = regexService;
        }

        [HttpPost]
        public async Task<ActionResult<List<string>>> CheckText(RegexModel regexModel)
        { 
            string text = regexModel.Text;
            List<string> data =  _regexService.CheckText(text);
            return Ok(data);
        }
    }
}
