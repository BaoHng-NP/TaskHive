using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.DTOs.Requests.User;
using TaskHive.Service.DTOs.Responses;
using Google.Apis.Auth;
using TaskHive.Service.Services.EmailService;
using TaskHive.Service.DTOs.Responses.User;
using AutoMapper;
using TaskHive.Repository.Repositories.ReviewRepository;

namespace TaskHive.Service.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IReviewRepository _reviewRepository;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService, IMapper mapper, IReviewRepository reviewRepository)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
            _mapper = mapper;
            _reviewRepository = reviewRepository;
        }

        // Generate refresh token
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        // Create AuthResponse without User info
        private async Task<AuthResponseDto> CreateAuthResponseAsync(User user)
        {
            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // Save refresh token to database
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.UserId,
                Token = refreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                Message = "Authentication successful!"
            };
        }

        public async Task<(AuthResponseDto? authResponse, string? errorMessage)> LoginAsync(LoginRequestDto model)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(model.Email);

                if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !VerifyPassword(model.Password, user.PasswordHash))
                {
                    // Check if user exists and has GoogleId (registered via Google)
                    if (user != null && !string.IsNullOrEmpty(user.GoogleId))
                    {
                        return (null, "This account was registered with Google. Please login with Google or reset your password to set a new one.");
                    }
                    return (null, "Invalid email or password.");
                }

                if (!user.IsEmailVerified)
                {
                    return (null, "Please verify your email before logging in.");
                }

                var authResponse = await CreateAuthResponseAsync(user);
                authResponse.Message = "Login successful!";

                return (authResponse, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                return (null, "Login failed. Please try again.");
            }
        }

        public async Task<(AuthResponseDto? authResponse, string? errorMessage)> RegisterFreelancerAsync(FreelancerRegisterRequestDto model)
        {
            //using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                if (await _unitOfWork.Users.ExistsByEmailAsync(model.Email))
                {
                    return (null, "Email already exists.");
                }

                var freelancer = new Freelancer
                {
                    Email = model.Email,
                    FullName = model.FullName,
                    Country = model.Country,
                    PasswordHash = HashPassword(model.Password),
                    PortfolioUrl = model.PortfolioUrl,
                    Role = Repository.Enums.UserRole.Freelancer,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsEmailVerified = false,
                    RemainingSlots = 0
                };

                await _unitOfWork.Users.AddFreelancerAsync(freelancer);
                await _unitOfWork.SaveChangesAsync();

                if (model.SkillIds?.Any() == true)
                {
                    foreach (var categoryId in model.SkillIds)
                    {
                        var userSkill = new UserSkill
                        {
                            UserId = freelancer.UserId,
                            CategoryId = categoryId,
                        };
                        await _unitOfWork.UserSkills.AddAsync(userSkill);
                    }
                }
                await _unitOfWork.SaveChangesAsync();

                await SendVerificationEmailAsync(freelancer);

                var authResponse = await CreateAuthResponseAsync(freelancer);
                authResponse.Message = "Registration successful! Please check your email to verify your account. If you don't see the email, please check your spam folder.";

                return (authResponse, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Freelancer registration failed: {ex.Message}");
                return (null, "Registration failed. Please try again.");
            }
        }

        public async Task<(AuthResponseDto? authResponse, string? errorMessage)> RegisterClientAsync(ClientRegisterRequestDto model)
        {
            try
            {
                if (await _unitOfWork.Users.ExistsByEmailAsync(model.Email))
                {
                    return (null, "Email already exists.");
                }

                var client = new Client
                {
                    Email = model.Email,
                    FullName = model.FullName,
                    Country = model.Country,
                    PasswordHash = HashPassword(model.Password),
                    Role = Repository.Enums.UserRole.Client,
                    CompanyName = "",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsEmailVerified = false
                };

                await _unitOfWork.Users.AddClientAsync(client);
                await _unitOfWork.SaveChangesAsync();

                await SendVerificationEmailAsync(client);

                var authResponse = await CreateAuthResponseAsync(client);
                authResponse.Message = "Registration successful! Please check your email to verify your account.  If you don't see the email, please check your spam folder.";

                return (authResponse, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client registration failed: {ex.Message}");
                return (null, "Registration failed. Please try again.");
            }
        }

        public async Task<(AuthResponseDto? authResponse, string? errorMessage)> GoogleLoginAsync(GoogleLoginRequestDto model)
        {
            try
            {
                var googleClientId = _configuration["Authentication:Google:ClientId"];
                if (string.IsNullOrEmpty(googleClientId))
                {
                    return (null, "Google Client ID is not configured.");
                }

                var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { googleClientId }
                });

                if (payload == null)
                {
                    return (null, "Invalid Google token.");
                }

                // Check existing user by Google ID
                var existingUserByGoogleId = await _unitOfWork.Users.GetByGoogleIdAsync(payload.Subject);
                if (existingUserByGoogleId != null)
                {
                    existingUserByGoogleId.FullName = payload.Name;
                    if (string.IsNullOrEmpty(existingUserByGoogleId.imageUrl))
                    {
                        existingUserByGoogleId.imageUrl = payload.Picture;
                    }
                    existingUserByGoogleId.IsEmailVerified = payload.EmailVerified;
                    existingUserByGoogleId.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.Users.UpdateAsync(existingUserByGoogleId);
                    await _unitOfWork.SaveChangesAsync();

                    var authResponse = await CreateAuthResponseAsync(existingUserByGoogleId);
                    authResponse.Message = "Google login successful!";

                    return (authResponse, null);
                }

                // Check existing user by email
                var existingUserByEmail = await _unitOfWork.Users.GetByEmailAsync(payload.Email);

                if (existingUserByEmail != null)
                {
                    existingUserByEmail.GoogleId = payload.Subject;
                    if (string.IsNullOrEmpty(existingUserByEmail.imageUrl))
                    {
                        existingUserByEmail.imageUrl = payload.Picture;
                    }
                    existingUserByEmail.IsEmailVerified = payload.EmailVerified;
                    existingUserByEmail.FullName = payload.Name;
                    existingUserByEmail.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.Users.UpdateAsync(existingUserByEmail);
                    await _unitOfWork.SaveChangesAsync();

                    var authResponse = await CreateAuthResponseAsync(existingUserByEmail);
                    authResponse.Message = "Google account linked successfully!";

                    return (authResponse, null);
                }

                // User does not exist, return special message for frontend to handle Google register
                return (null, "GOOGLE_REGISTER_REQUIRED");
            }
            catch (InvalidJwtException)
            {
                return (null, "Invalid Google token format.");
            }
            catch (Exception ex)
            {
                return (null, $"Google login failed: {ex.Message}");
            }
        }

        public async Task<(AuthResponseDto? authResponse, string? errorMessage)> GoogleRegisterAsync(GoogleRegisterRequestDto model)
        {
            try
            {
                var googleClientId = _configuration["Authentication:Google:ClientId"];
                if (string.IsNullOrEmpty(googleClientId))
                {
                    return (null, "Google Client ID is not configured.");
                }

                var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { googleClientId }
                });

                if (payload == null)
                {
                    return (null, "Invalid Google token.");
                }

                if (await _unitOfWork.Users.ExistsByEmailAsync(payload.Email))
                {
                    return (null, "Email already exists. Please use regular login.");
                }

                if (!Enum.TryParse<Repository.Enums.UserRole>(model.Role, out var userRole))
                {
                    return (null, "Invalid role specified.");
                }

                User newUser;

                if (userRole == Repository.Enums.UserRole.Freelancer)
                {
                    var freelancer = new Freelancer
                    {
                        Email = payload.Email,
                        FullName = payload.Name,
                        Country = model.Country,
                        GoogleId = payload.Subject,
                        imageUrl = payload.Picture,
                        PortfolioUrl = model.PortfolioUrl,
                        Role = Repository.Enums.UserRole.Freelancer,
                        IsEmailVerified = payload.EmailVerified,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        PasswordHash = null,
                        RemainingSlots = 0
                    };

                    await _unitOfWork.Users.AddFreelancerAsync(freelancer);
                    await _unitOfWork.SaveChangesAsync();


                    if (model.SkillIds?.Any() == true)
                    {
                        foreach (var categoryId in model.SkillIds)
                        {
                            var userSkill = new UserSkill
                            {
                                UserId = freelancer.UserId,
                                CategoryId = categoryId,
                            };
                            await _unitOfWork.UserSkills.AddAsync(userSkill);
                        }
                    }
                    await _unitOfWork.SaveChangesAsync();

                    newUser = freelancer;
                }
                else
                {
                    var client = new Client
                    {
                        Email = payload.Email,
                        FullName = payload.Name,
                        Country = model.Country,
                        GoogleId = payload.Subject,
                        imageUrl = payload.Picture,
                        Role = Repository.Enums.UserRole.Client,
                        CompanyName = "",
                        IsEmailVerified = payload.EmailVerified,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        PasswordHash = null
                    };

                    await _unitOfWork.Users.AddClientAsync(client);
                    await _unitOfWork.SaveChangesAsync();

                    newUser = client;
                }

                var authResponse = await CreateAuthResponseAsync(newUser);
                authResponse.Message = $"Google {model.Role} registration successful!";

                return (authResponse, null);
            }
            catch (InvalidJwtException)
            {
                return (null, "Invalid Google token format.");
            }
            catch (Exception ex)
            {
                return (null, $"Google registration failed: {ex.Message}");
            }
        }

        public async Task<(AuthResponseDto? authResponse, string? errorMessage)> RefreshTokenAsync(RefreshTokenRequestDto model)
        {
            try
            {
                var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(model.RefreshToken);

                if (refreshToken == null)
                {
                    return (null, "Invalid or expired refresh token.");
                }

                var user = refreshToken.User;

                // Revoke old refresh token
                refreshToken.IsRevoked = true;
                await _unitOfWork.RefreshTokens.UpdateAsync(refreshToken);

                // Create new tokens
                var authResponse = await CreateAuthResponseAsync(user);
                authResponse.Message = "Token refreshed successfully!";

                return (authResponse, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Refresh token failed: {ex.Message}");
                return (null, "Failed to refresh token. Please login again.");
            }
        }

        public async Task<bool> LogoutAsync(int userId)
        {
            try
            {
                await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(userId);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout failed: {ex.Message}");
                return false;
            }
        }


        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            string hashedEnteredPassword = HashPassword(enteredPassword);
            return hashedEnteredPassword == hashedPassword;
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                throw new InvalidOperationException("JWT configuration is incomplete.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<(EmailVerificationResponseDto response, string? errorMessage)> VerifyEmailAsync(EmailVerificationRequestDto model)
        {
            try
            {
                var verificationToken = await _unitOfWork.EmailVerifications.GetByTokenAsync(model.Token);

                if (verificationToken == null)
                {
                    return (new EmailVerificationResponseDto
                    {
                        Success = false,
                        Message = "Invalid or expired verification token."
                    }, null);
                }

                var user = verificationToken.User;
                user.IsEmailVerified = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);

                verificationToken.IsUsed = true;
                await _unitOfWork.EmailVerifications.UpdateAsync(verificationToken);

                await _unitOfWork.SaveChangesAsync();

                await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName);

                return (new EmailVerificationResponseDto
                {
                    Success = true,
                    Message = "Email verified successfully! Your account is now active.",
                    RedirectUrl = "/dashboard"
                }, null);
            }
            catch (Exception ex)
            {
                return (new EmailVerificationResponseDto
                {
                    Success = false,
                    Message = "An error occurred during email verification."
                }, $"Email verification failed: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> ResendVerificationEmailAsync(ResendVerificationRequestDto model)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(model.Email);

                if (user == null)
                {
                    return (false, "User not found.");
                }

                if (user.IsEmailVerified)
                {
                    return (false, "Email is already verified.");
                }

                if (await _unitOfWork.EmailVerifications.HasValidTokenAsync(user.UserId))
                {
                    return (false, "A verification email has already been sent. Please check your inbox.  If you don't see the email, please check your spam folder.");
                }

                await SendVerificationEmailAsync(user);

                return (true, "Verification email sent successfully. Please check your inbox.  If you don't see the email, please check your spam folder.");
            }
            catch (Exception ex)
            {
                return (false, "Failed to send verification email. Please try again later.");
            }
        }

        public async Task<(bool success, string message)> RequestPasswordResetAsync(PasswordResetRequestDto model)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(model.Email);

                if (user == null)
                {
                    return (true, "If an account with that email exists, a password reset link has been sent.");
                }

                if (await _unitOfWork.EmailVerifications.HasValidPasswordResetTokenAsync(user.UserId))
                {
                    return (false, "A password reset email has already been sent. Please check your inbox.  If you don't see the email, please check your spam folder.");
                }

                var token = GenerateSecureToken();

                var resetToken = new EmailVerificationToken
                {
                    UserId = user.UserId,
                    Token = token,
                    Email = user.Email,
                    IsPasswordReset = true,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    IsUsed = false
                };

                await _unitOfWork.EmailVerifications.AddAsync(resetToken);
                await _unitOfWork.SaveChangesAsync();

                await _emailService.SendPasswordResetEmailAsync(user.Email, user.FullName, token);

                return (true, "Password reset link has been sent to your email.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Password reset request failed: {ex.Message}");
                return (false, "Failed to process request. Please try again.");
            }
        }

        public async Task<(EmailVerificationResponseDto response, string? errorMessage)> ResetPasswordAsync(ResetPasswordRequestDto model)
        {
            try
            {
                var resetToken = await _unitOfWork.EmailVerifications.GetPasswordResetTokenAsync(model.Token);

                if (resetToken == null)
                {
                    return (new EmailVerificationResponseDto
                    {
                        Success = false,
                        Message = "Invalid or expired password reset token."
                    }, null);
                }

                var user = resetToken.User;
                user.PasswordHash = HashPassword(model.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);

                resetToken.IsUsed = true;
                await _unitOfWork.EmailVerifications.UpdateAsync(resetToken);

                await _unitOfWork.EmailVerifications.InvalidatePasswordResetTokensAsync(user.UserId);

                await _unitOfWork.SaveChangesAsync();

                return (new EmailVerificationResponseDto
                {
                    Success = true,
                    Message = "Password reset successfully! You can now login with your new password.",
                    RedirectUrl = "/login"
                }, null);
            }
            catch (Exception ex)
            {
                return (new EmailVerificationResponseDto
                {
                    Success = false,
                    Message = "An error occurred while resetting password."
                }, $"Password reset failed: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> ChangePasswordAsync(string userId, ChangePasswordRequestDto model)
        {
            try
            {
                if (!int.TryParse(userId, out var userIdInt))
                {
                    return (false, "Invalid user ID.");
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userIdInt);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                if (!string.IsNullOrEmpty(user.PasswordHash) &&
                    !VerifyPassword(model.CurrentPassword, user.PasswordHash))
                {
                    return (false, "Current password is incorrect.");
                }

                user.PasswordHash = HashPassword(model.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);

                await _unitOfWork.EmailVerifications.InvalidatePasswordResetTokensAsync(user.UserId);

                await _unitOfWork.SaveChangesAsync();

                return (true, "Password changed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Change password failed: {ex.Message}");
                return (false, "Failed to change password. Please try again.");
            }
        }

        private async Task SendVerificationEmailAsync(User user)
        {
            var token = GenerateSecureToken();

            var verificationToken = new EmailVerificationToken
            {
                UserId = user.UserId,
                Token = token,
                Email = user.Email,
                IsPasswordReset = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                IsUsed = false
            };

            await _unitOfWork.EmailVerifications.AddAsync(verificationToken);
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendVerificationEmailAsync(user.Email, user.FullName, token);
        }

        private string GenerateSecureToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var tokenBytes = new byte[32];
            rng.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }



        //User CRUD

        public async Task<(Freelancer? Freelancer, int Count, double AverageRating)> GetFreelancerByIdAsync(int userId)
        {
            var reviews = await _reviewRepository.GetByRevieweeIdAsync(userId);
            int count = reviews.Count;
            double averageRating = count > 0 ? reviews.Average(r => r.Rating) : 0.0;

            var freelancer = await _unitOfWork.Users.GetFreelancerByIdAsync(userId);

            return (freelancer, count, averageRating);
        }
        public async Task<(Client? Client, int Count, double AverageRating)> GetClientByIdAsync(int userId)
        {
            var reviews = await _reviewRepository.GetByRevieweeIdAsync(userId);
            int count = reviews.Count;
            double averageRating = count > 0 ? reviews.Average(r => r.Rating) : 0.0;
            var client = await _unitOfWork.Users.GetClientByIdAsync(userId);
            return (client, count, averageRating);
        }

        public async Task<(FreelancerProfileResponseDto? freelancerProfileResponse, string? errorMessage)> UpdateFreelancerProfileAsync(int userId, FreelancerProfileDto model)
        {
            try
            {
                var freelancer = await _unitOfWork.Users.GetFreelancerByIdAsync(userId);
                if (freelancer == null)
                {
                    return (null, "User not found.");
                }

                _mapper.Map(model, freelancer);

                freelancer.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(freelancer);
                await _unitOfWork.SaveChangesAsync();

                FreelancerProfileResponseDto response = new();
                _mapper.Map(freelancer, response);


                return (response, null);
            }
            catch (Exception ex)
            {
                return (null, $"Failed to update freelancer profile: {ex.Message}");
            }
        }

        public async Task<(ClientProfileResponseDto? clientProfileResponse, string? errorMessage)> UpdateClientProfileAsync(int userId, ClientProfileDto model)
        {
            try
            {
                var client = await _unitOfWork.Users.GetClientByIdAsync(userId);
                if (client == null)
                {
                    return (null, "User not found.");
                }

                _mapper.Map(model, client);

                client.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(client);
                await _unitOfWork.SaveChangesAsync();

                ClientProfileResponseDto response = new();
                _mapper.Map(client, response);

                // Print all fields of client
                Console.WriteLine("Client fields:");
                foreach (var prop in client.GetType().GetProperties())
                {
                    Console.WriteLine($"{prop.Name}: {prop.GetValue(client)}");
                }

                // Print all fields of response
                Console.WriteLine("ClientProfileResponseDto fields:");
                foreach (var prop in response.GetType().GetProperties())
                {
                    Console.WriteLine($"{prop.Name}: {prop.GetValue(response)}");
                }

                return (response, null);
            }
            catch (Exception ex)
            {
                return (null, $"Failed to update client profile: {ex.Message}");
            }
        }


        public async Task<(FreelancerProfileResponseDto? freelancerProfileResponse, string? errorMessage)> UpdateRemainingSlotAsync(int userId, int UpdateSlot)
        {
            try
            {
                var freelancer = await _unitOfWork.Users.GetFreelancerByIdAsync(userId);
                if (freelancer == null)
                {
                    return (null, "User not found.");
                }

                freelancer.RemainingSlots = UpdateSlot;

                freelancer.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(freelancer);
                await _unitOfWork.SaveChangesAsync();

                FreelancerProfileResponseDto response = new();
                _mapper.Map(freelancer, response);


                return (response, null);
            }
            catch (Exception ex)
            {
                return (null, $"Failed to update remaining slot profile: {ex.Message}");
            }
        }

    }
}
