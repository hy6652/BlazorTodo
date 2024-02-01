using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BlazorTodo.Shared;
using BlazorTodo.Server.Services.Utility;

namespace BlazorTodo.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CsvController : ControllerBase
    {
        private CsvService _csvService;

        public CsvController(CsvService csvService)
        {
            _csvService = csvService;
        }

        [HttpPost]
        public async Task<ActionResult> WriteCsv([FromBody] CsvDto csvDto)
        {
            await _csvService.WriteCsv(csvDto);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<CsvModel>>> ReadCsv()
        {
            var result = await _csvService.ReadCsv();
            return result;
        }

        [HttpPost]
        public async Task<ActionResult<List<CsvModel>>> ReadSelectedCsv([FromForm] IFormFile file)
        {
            Stream stream = file.OpenReadStream();
            var result = await _csvService.ReadSelectedCsv(stream);
            return result;
        }
    }
}
