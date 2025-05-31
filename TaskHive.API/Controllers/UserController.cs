using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskHive.Service.DTOs.Requests.User;
using TaskHive.Service.Services.UserService;

namespace TaskHive.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(TaskHive.Service.DTOs.Responses.AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (authResponse, errorMessage) = await _userService.RegisterAsync(model);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(authResponse);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(TaskHive.Service.DTOs.Responses.AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (authResponse, errorMessage) = await _userService.LoginAsync(model);

            if (errorMessage != null)
            {
                return Unauthorized(new { message = errorMessage });
            }

            return Ok(authResponse);
        }
    }

}
