using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Service.DTOs.Requests.User;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.UserService
{
    public interface IUserService
    {
        Task<(AuthResponseDto? authResponse, string? errorMessage)> LoginAsync(LoginRequestDto model);

        Task<(AuthResponseDto? authResponse, string? errorMessage)> RegisterFreelancerAsync(FreelancerRegisterRequestDto model);
        Task<(AuthResponseDto? authResponse, string? errorMessage)> RegisterClientAsync(ClientRegisterRequestDto model);

        Task<(AuthResponseDto? authResponse, string? errorMessage)> GoogleLoginAsync(GoogleLoginRequestDto model);

        Task<(AuthResponseDto? authResponse, string? errorMessage)> GoogleRegisterAsync(GoogleRegisterRequestDto model);

        Task<(EmailVerificationResponseDto? response, string? errorMessage)> VerifyEmailAsync(EmailVerificationRequestDto model);

        Task<(bool success, string message)> ResendVerificationEmailAsync(ResendVerificationRequestDto model);

        Task<(bool success, string message)> RequestPasswordResetAsync(PasswordResetRequestDto model);
        Task<(EmailVerificationResponseDto response, string? errorMessage)> ResetPasswordAsync(ResetPasswordRequestDto model);
        Task<(bool success, string message)> ChangePasswordAsync(string userId, ChangePasswordRequestDto model);

        Task<(AuthResponseDto? authResponse, string? errorMessage)> RefreshTokenAsync(RefreshTokenRequestDto model);
        Task<bool> LogoutAsync(int userId);

        Task<Freelancer?> GetFreelancerByIdAsync(int userId);

        Task<Client?> GetClientByIdAsync(int userId);



    }
}
