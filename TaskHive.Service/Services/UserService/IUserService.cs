using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Service.DTOs.Requests.User;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.UserService
{
    public interface IUserService
    {
        Task<(AuthResponseDto? authResponse, string? errorMessage)> RegisterAsync(RegisterRequestDto model);
        Task<(AuthResponseDto? authResponse, string? errorMessage)> LoginAsync(LoginRequestDto model);
    }
}
