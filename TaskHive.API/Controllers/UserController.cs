using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        [HttpPost("register/freelancer")]
        [ProducesResponseType(typeof(TaskHive.Service.DTOs.Responses.AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterFreelancer(FreelancerRegisterRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (authResponse, errorMessage) = await _userService.RegisterFreelancerAsync(model);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(authResponse);
        }

        [HttpPost("register/client")]
        [ProducesResponseType(typeof(TaskHive.Service.DTOs.Responses.AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterClient(ClientRegisterRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (authResponse, errorMessage) = await _userService.RegisterClientAsync(model);

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

        [HttpPost("google-login")]
        [ProducesResponseType(typeof(TaskHive.Service.DTOs.Responses.AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GoogleLogin(GoogleLoginRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (authResponse, errorMessage) = await _userService.GoogleLoginAsync(model);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(authResponse);
        }

        [HttpPost("google-register")]
        [ProducesResponseType(typeof(TaskHive.Service.DTOs.Responses.AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GoogleRegister(GoogleRegisterRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (authResponse, errorMessage) = await _userService.GoogleRegisterAsync(model);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(authResponse);
        }


        [HttpPost("verify-email")]
        [ProducesResponseType(typeof(TaskHive.Service.DTOs.Responses.EmailVerificationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyEmail(EmailVerificationRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var (response, errorMessage) = await _userService.VerifyEmailAsync(model);
            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }
            return Ok(response);
        }

        [HttpPost("resend-verification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResendVerificationEmail(ResendVerificationRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message) = await _userService.ResendVerificationEmailAsync(model);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        [HttpPost("request-password-reset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestPasswordReset(PasswordResetRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message) = await _userService.RequestPasswordResetAsync(model);

            return Ok(new { success, message });
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(TaskHive.Service.DTOs.Responses.EmailVerificationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (response, errorMessage) = await _userService.ResetPasswordAsync(model);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(response);
        }

        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var (success, message) = await _userService.ChangePasswordAsync(userId, model);

            if (!success)
            {
                return BadRequest(new { success = false, message });
            }

            return Ok(new { success = true, message });
        }

    }
}
