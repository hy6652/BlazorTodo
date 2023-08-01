using BlazorTodo.Server.Services.DIexample;
using Microsoft.AspNetCore.Mvc;
using BlazorTodo.Shared;

namespace BlazorTodo.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserDiController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserDiController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<List<User>> GetAll()
        {
            return await _userRepository.GetAll();
        }
    }
}
