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


namespace TaskHive.Service.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
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

        public async Task<(AuthResponseDto? authResponse, string? errorMessage)> RegisterAsync(RegisterRequestDto model)
        {
            if (await _unitOfWork.Users.ExistsByEmailAsync(model.Email))
            {
                return (null, "Email already exists.");
            }

            var user = new User
            {
                Email = model.Email,
                FullName = model.FullName,
                PasswordHash = HashPassword(model.Password),
                Role = Repository.Enums.UserRole.Freelancer,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsEmailVerified = false
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return (new AuthResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                ImageUrl = user.imageUrl,
                Token = GenerateJwtToken(user),
                Message = "Registration successful!"
            }, null);
        }

        public async Task<(AuthResponseDto? authResponse, string? errorMessage)> LoginAsync(LoginRequestDto model)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(model.Email);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !VerifyPassword(model.Password, user.PasswordHash))
            {
                return (null, "Invalid email or password.");
            }

            var token = GenerateJwtToken(user);
            return (new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                ImageUrl = user.imageUrl,
                Message = "Login successful!"
            }, null);
        }

        public async Task<(AuthResponseDto? authResponse, string? errorMessage)> GoogleLoginAsync(GoogleLoginRequestDto model)
        {
            try
            {
                // Lấy Google Client ID từ configuration 
                var googleClientId = _configuration["Authentication:Google:ClientId"];
                if (string.IsNullOrEmpty(googleClientId))
                {
                    return (null, "Google Client ID is not configured.");
                }

                // Verify Google ID Token
                var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { googleClientId }
                });

                if (payload == null)
                {
                    return (null, "Invalid Google token.");
                }

                // Kiểm tra user đã tồn tại 
                var existingUserByGoogleId = await _unitOfWork.Users.GetByGoogleIdAsync(payload.Subject);

                if (existingUserByGoogleId != null)
                {
                    // User đã tồn tại với Google ID, cập nhật thông tin và trả về token
                    existingUserByGoogleId.FullName = payload.Name;
                    existingUserByGoogleId.imageUrl = payload.Picture;
                    existingUserByGoogleId.IsEmailVerified = payload.EmailVerified;
                    existingUserByGoogleId.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.Users.UpdateAsync(existingUserByGoogleId);
                    await _unitOfWork.SaveChangesAsync();

                    var token = GenerateJwtToken(existingUserByGoogleId);
                    return (new AuthResponseDto
                    {
                        Token = token,
                        UserId = existingUserByGoogleId.UserId,
                        Email = existingUserByGoogleId.Email,
                        FullName = existingUserByGoogleId.FullName,
                        Role = existingUserByGoogleId.Role.ToString(),
                        ImageUrl = existingUserByGoogleId.imageUrl,
                        Message = "Google login successful!"
                    }, null);
                }

                // Kiểm tra user đã tồn tại với email chưa
                var existingUserByEmail = await _unitOfWork.Users.GetByEmailAsync(payload.Email);

                if (existingUserByEmail != null)
                {
                    // User tồn tại với email, link Google account
                    existingUserByEmail.GoogleId = payload.Subject;
                    existingUserByEmail.imageUrl = payload.Picture;
                    existingUserByEmail.IsEmailVerified = payload.EmailVerified;
                    existingUserByEmail.FullName = payload.Name; // Cập nhật tên từ Google
                    existingUserByEmail.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.Users.UpdateAsync(existingUserByEmail);
                    await _unitOfWork.SaveChangesAsync();

                    var token = GenerateJwtToken(existingUserByEmail);
                    return (new AuthResponseDto
                    {
                        Token = token,
                        UserId = existingUserByEmail.UserId,
                        Email = existingUserByEmail.Email,
                        FullName = existingUserByEmail.FullName,
                        Role = existingUserByEmail.Role.ToString(),
                        ImageUrl = existingUserByEmail.imageUrl,
                        Message = "Google account linked successfully!"
                    }, null);
                }

                var newUser = new User
                {
                    Email = payload.Email,
                    FullName = payload.Name,
                    GoogleId = payload.Subject,
                    imageUrl = payload.Picture,
                    Role = Repository.Enums.UserRole.Freelancer,
                    IsEmailVerified = payload.EmailVerified,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PasswordHash = null
                };

                await _unitOfWork.Users.AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();

                var newUserToken = GenerateJwtToken(newUser);
                return (new AuthResponseDto
                {
                    Token = newUserToken,
                    UserId = newUser.UserId,
                    Email = newUser.Email,
                    FullName = newUser.FullName,
                    Role = newUser.Role.ToString(),
                    ImageUrl = newUser.imageUrl,
                    Message = "Google account created successfully!"
                }, null);
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

        public async Task<(AuthResponseDto? authResponse, string? errorMessage)> RegisterFreelancerAsync(FreelancerRegisterRequestDto model)
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
                RemainingSlots = 0 // Default slots
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
                await _unitOfWork.SaveChangesAsync();
            }

            await SendVerificationEmailAsync(freelancer);

            return (new AuthResponseDto
            {
                UserId = freelancer.UserId,
                Email = freelancer.Email,
                FullName = freelancer.FullName,
                Role = freelancer.Role.ToString(),
                ImageUrl = freelancer.imageUrl,
                Token = GenerateJwtToken(freelancer),
                Message = "Registration successful! Please check your email to verify your account."
            }, null);
        }

        public async Task<(AuthResponseDto? authResponse, string? errorMessage)> RegisterClientAsync(ClientRegisterRequestDto model)
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

            return (new AuthResponseDto
            {
                UserId = client.UserId,
                Email = client.Email,
                FullName = client.FullName,
                Role = client.Role.ToString(),
                ImageUrl = client.imageUrl,
                Token = GenerateJwtToken(client),
                Message = "Registration successful! Please check your email to verify your account."
            }, null);
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
                        RemainingSlots = 3
                    };

                    await _unitOfWork.Users.AddFreelancerAsync(freelancer);
                    await _unitOfWork.SaveChangesAsync();

                    if (model.SkillIds?.Any() == true)
                    {
                        foreach (var categoryId in model.SkillIds) // Đây thực ra là CategoryId
                        {
                            var userSkill = new UserSkill
                            {
                                UserId = freelancer.UserId,
                                CategoryId = categoryId, // Sử dụng CategoryId
                            };
                            await _unitOfWork.UserSkills.AddAsync(userSkill);
                        }
                        await _unitOfWork.SaveChangesAsync();
                    }

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

                var token = GenerateJwtToken(newUser);
                return (new AuthResponseDto
                {
                    Token = token,
                    UserId = newUser.UserId,
                    Email = newUser.Email,
                    FullName = newUser.FullName,
                    Role = newUser.Role.ToString(),
                    ImageUrl = newUser.imageUrl,
                    Message = $"Google {model.Role} registration successful!"
                }, null);
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
                Expires = DateTime.UtcNow.AddDays(7), // Token có hiệu lực 7 ngày
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

                // Update user email verification status
                var user = verificationToken.User;
                user.IsEmailVerified = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);

                // Mark token as used
                verificationToken.IsUsed = true;
                await _unitOfWork.EmailVerifications.UpdateAsync(verificationToken);

                await _unitOfWork.SaveChangesAsync();

                // Send welcome email
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

                // Check if user already has a valid token
                if (await _unitOfWork.EmailVerifications.HasValidTokenAsync(user.UserId))
                {
                    return (false, "A verification email has already been sent. Please check your inbox.");
                }

                // Send new verification email
                await SendVerificationEmailAsync(user);

                return (true, "Verification email sent successfully. Please check your inbox.");
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

                // Check if user already has a valid password reset token
                if (await _unitOfWork.EmailVerifications.HasValidPasswordResetTokenAsync(user.UserId))
                {
                    return (false, "A password reset email has already been sent. Please check your inbox.");
                }

                // Generate reset token
                var token = GenerateSecureToken();

                var resetToken = new EmailVerificationToken
                {
                    UserId = user.UserId,
                    Token = token,
                    Email = user.Email,
                    IsPasswordReset = true, // Đây là password reset token
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(1), // 1 hour expiry
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
                // Tìm password reset token
                var resetToken = await _unitOfWork.EmailVerifications.GetPasswordResetTokenAsync(model.Token);

                if (resetToken == null)
                {
                    return (new EmailVerificationResponseDto
                    {
                        Success = false,
                        Message = "Invalid or expired password reset token."
                    }, null);
                }

                // Update password
                var user = resetToken.User;
                user.PasswordHash = HashPassword(model.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);

                // Mark token as used
                resetToken.IsUsed = true;
                await _unitOfWork.EmailVerifications.UpdateAsync(resetToken);

                // Invalidate all other password reset tokens
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

                // Verify current password
                if (!string.IsNullOrEmpty(user.PasswordHash) &&
                    !VerifyPassword(model.CurrentPassword, user.PasswordHash))
                {
                    return (false, "Current password is incorrect.");
                }

                // Update password
                user.PasswordHash = HashPassword(model.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);

                // Invalidate all password reset tokens
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

        // Cập nhật existing method để set IsPasswordReset = false
        private async Task SendVerificationEmailAsync(User user)
        {
            var token = GenerateSecureToken();

            var verificationToken = new EmailVerificationToken
            {
                UserId = user.UserId,
                Token = token,
                Email = user.Email,
                IsPasswordReset = false, // Email verification token
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






    }
}
