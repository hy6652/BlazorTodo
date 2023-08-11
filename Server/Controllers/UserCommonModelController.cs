using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorTodo.Server.Services.DIexampleWithCommonModel;
using BlazorTodo.Shared;
using System.Diagnostics;

namespace BlazorTodo.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserCommonModelController : ControllerBase
    {
        private readonly ICommonRepository<UserWithCommonModel> _userCommonRepository;

        public UserCommonModelController(ICommonRepository<UserWithCommonModel> userCommonRepository)
        {
            _userCommonRepository = userCommonRepository;
        }

        [HttpGet]
        public async Task<ActionResult<UserWithCommonModel>> AddModelAsync()
        {
            UserWithCommonModel user = new();
            var item = await _userCommonRepository.AddAsync(user);
            return Ok();
        }
    }
}