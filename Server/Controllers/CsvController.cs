using Microsoft.AspNetCore.Mvc;
using BlazorTodo.Server.Services;
using CsvHelper;
using System.Globalization;
using System.Diagnostics;
using BlazorTodo.Shared;

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
        public async Task<ActionResult> WriteCsv(List<CsvModel> data)
        {
            var result = await _csvService.WriteCsv(data);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<CsvModel>>> ReadCsv()
        {
            var result = await _csvService.ReadCsv();
            return result;
        }
    }
}
