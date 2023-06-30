using Microsoft.AspNetCore.Mvc;
using BlazorTodo.Server.Services;
using BlazorTodo.Shared;

// TEST Controller
namespace BlazorTodo.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private BlobImageService _blobService;

        public FileController(BlobImageService blobService)
        {
            _blobService = blobService;
        }

        [HttpPost]
        public async Task<ActionResult<ImageUpload>> UploadImage([FromForm] IFormFile file)
        {
            Stream stream = file.OpenReadStream();
            string fileName = file.FileName;
            await _blobService.UploadImage(stream, fileName);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<ImageUpload>>> GetImages()
        {
            var result = await _blobService.GetImages();
            return result;
        }
    }
}
