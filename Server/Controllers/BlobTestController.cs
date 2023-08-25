using BlazorTodo.Server.Services;
using BlazorTodo.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlazorTodo.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BlobTestController : ControllerBase
    {
        private BlobTestService _blobTestService;
        private CognitiveSearchService _cognitiveSearchService;

        public BlobTestController(BlobTestService blobTestService, CognitiveSearchService cognitiveSearchService)
        {
            _blobTestService = blobTestService;
            _cognitiveSearchService = cognitiveSearchService;
        }

        [HttpGet]
        public async Task<ActionResult> DownloadExcel()
        {
            await _blobTestService.DownloadExcel();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> PushExcelToSearch()
        {
            var blocks = await _blobTestService.GetExcelFromBlob();
            Debug.WriteLine($"blocks : {blocks.Count}");
            await _cognitiveSearchService.PushBlockToSearch(blocks);
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> CreateIndex()
        {
            await _cognitiveSearchService.CreateIndex();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<List<SearchResult>>> Search(SearchMessage message)
        {
            var result = await _cognitiveSearchService.SearchKnowledgebase(message.Question);
            return Ok(result);
        }
    }
}
