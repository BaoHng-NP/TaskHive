using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Service.DTOs.Requests.User;
using TaskHive.Service.DTOs.Responses.User;
using TaskHive.Service.Services.UserService;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
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

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(TaskHive.Service.DTOs.Responses.AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (authResponse, errorMessage) = await _userService.RefreshTokenAsync(model);

            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(authResponse);
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out var userIdInt))
            {
                await _userService.LogoutAsync(userIdInt);
            }

            return Ok(new { message = "Logged out successfully" });
        }


        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userIdInt))
            {
                return Unauthorized(new { message = "Invalid user ID" });
            }
            try
            {
                if (string.Equals(userRole, "Freelancer", StringComparison.OrdinalIgnoreCase))
                {
                    var freelancer = await _userService.GetFreelancerByIdAsync(userIdInt);
                    if (freelancer == null) return NotFound(new { message = "User not found" });
                    var response = _mapper.Map<FreelancerProfileResponseDto>(freelancer);
                    return Ok(response);
                }
                else if (string.Equals(userRole, "Client", StringComparison.OrdinalIgnoreCase))
                {
                    var client = await _userService.GetClientByIdAsync(userIdInt);
                    if (client == null) return NotFound(new { message = "User not found" });
                    var response = _mapper.Map<ClientProfileResponseDto>(client);
                    return Ok(response);
                }
                else
                {
                    return Unauthorized(new { message = "Invalid user role" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("freelancer/{userId}")]
        //[Authorize(Roles = "Freelancer,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> GetFreelancerProfile(int userId)
        {
            try
            {
                var freelancer = await _userService.GetFreelancerByIdAsync(userId);
                if (freelancer == null) return NotFound(new { message = $"Cannot fetch user profile" });
                var response = _mapper.Map<FreelancerProfileResponseDto>(freelancer);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        [HttpGet("client/{userId}")]
        //[Authorize(Roles = "Client,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClientProfile(int userId)
        {
            try
            {
                var client = await _userService.GetClientByIdAsync(userId);
                if (client == null) return NotFound(new { message = $"Cannot fetch user profile" });
                var response = _mapper.Map<ClientProfileResponseDto>(client);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpPut("freelancer/{userId}")]
        public async Task<IActionResult> UpdateFreelancerProfile(int userId, FreelancerProfileDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var (freelancerProfileResponse, errorMessage) = await _userService.UpdateFreelancerProfileAsync(userId, model);
            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }
            return Ok(freelancerProfileResponse);
        }


        [HttpPut("client/{userId}")]
        public async Task<IActionResult> UpdateClientProfile(int userId, ClientProfileDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var (clientProfileResponse, errorMessage) = await _userService.UpdateClientProfileAsync(userId, model);
            if (errorMessage != null)
            {
                return BadRequest(new { message = errorMessage });
            }
            return Ok(clientProfileResponse);
        }
    }
}
