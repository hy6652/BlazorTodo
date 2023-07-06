﻿using Microsoft.AspNetCore.Mvc;
using BlazorTodo.Server.Services;
using BlazorTodo.Shared;
using System.Diagnostics;

namespace BlazorTodo.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private BlobImageService _blobService;
        private CsvService _csvService;
 
        public FileController(BlobImageService blobService, CsvService csvService)
        {
            _blobService = blobService;
            _csvService = csvService;
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


        [HttpPost]
        public async Task<ActionResult<string>> CreateImageSas(blobClass blob)
        {
            var blobName = blob.blobName;
            var sasUri =  await _blobService.CreateServiceSasForBlob(blobName);
            return sasUri.AbsoluteUri;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetCsvContainerSas()
        {
            var sasUri = _blobService.GetContainerSASUri();
            return sasUri.AbsoluteUri;
        }

        [HttpPost]
        public async Task<ActionResult> UploadCsv([FromForm] IFormFile file)
        {
            Stream stream = file.OpenReadStream();
            List<CsvModel> data = await _csvService.ReadSelectedCsv(file);
            List<CsvItem> csvItem = await _blobService.UploadCsv(file, data);
            return Ok(csvItem);
        }

        [HttpGet]
        public async Task<ActionResult<List<BlobTitleModel>>> GetAllCsvTitle()
        {
            List<BlobTitleModel> titles = await _blobService.GetAllCsvTitle();
            return titles;
        }

        [HttpGet]
        public async Task DownloadAllCsv()
        {
            await _blobService.DownloadCsv();
        }

        [HttpPost]
        public async Task<ActionResult> DownloadOneCsvByTitle(BlobTitleModel title)
        {
            await _blobService.DownloadOneCsvByTitle(title);
            return Ok();
        }
    }
}
