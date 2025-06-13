using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace TaskHive.Service.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendVerificationEmailAsync(string email, string fullName, string verificationToken)
        {
            try
            {
                var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";
                var verificationUrl = $"{frontendUrl}/verify-email?token={verificationToken}";

                var subject = "Verify Your TaskHive Account";
                var body = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Verify Your TaskHive Account</title>
                    </head>
                    <body style='margin: 0; padding: 0; background-color: #fffbeb;'>
                        <div style='font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif; max-width: 600px; margin: 0 auto; background-color: #ffffff; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);'>
                            
                            <!-- Header với Gradient Background -->
                            <div style='background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 50%, #d97706 100%); color: white; padding: 50px 20px; text-align: center; position: relative;'>
                                <!-- Simple Logo Icon -->
                                <div style='margin-bottom: 25px;'>
                                    <div style='display: inline-block; width: 90px; height: 90px; background: rgba(255,255,255,0.15); border-radius: 20px; position: relative; border: 3px solid rgba(255,255,255,0.3);'>
                                        <div style='position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); font-size: 45px;'>
                                            🐝
                                        </div>
                                    </div>
                                </div>
                                
                                <h1 style='margin: 0; font-size: 34px; font-weight: 700; text-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                                    Welcome to TaskHive!
                                </h1>
                                <p style='margin: 12px 0 0 0; font-size: 19px; opacity: 0.95; font-weight: 300;'>
                                    Where talent meets opportunity 
                                </p>
                            </div>
                            
                            <!-- Content Body -->
                            <div style='padding: 45px 35px; background-color: #ffffff;'>
                                <h2 style='color: #d97706; font-size: 26px; margin: 0 0 22px 0; font-weight: 600;'>
                                    Hi {fullName}! 👋
                                </h2>
                                
                                <p style='color: #374151; line-height: 1.7; margin: 0 0 22px 0; font-size: 17px;'>
                                    Thank you for joining the <strong style='color: #d97706;'>TaskHive</strong> community! To complete your registration and start connecting with amazing opportunities, please verify your email address.
                                </p>
                                
                                <div style='background: linear-gradient(135deg, #fef3c7, #fde68a); padding: 25px; border-radius: 15px; margin: 30px 0; border-left: 5px solid #fbbf24;'>
                                    <p style='margin: 0; color: #92400e; font-weight: 500; font-size: 16px;'>
                                        🚀 <strong>What's next?</strong> Click the button below to activate your account and start your freelancing journey!
                                    </p>
                                </div>
                                
                                <!-- CTA Button -->
                                <div style='text-align: center; margin: 40px 0;'>
                                    <a href='{verificationUrl}' 
                                       style='background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 50%, #d97706 100%); 
                                              color: white; 
                                              padding: 18px 45px; 
                                              text-decoration: none; 
                                              border-radius: 15px; 
                                              display: inline-block; 
                                              font-weight: 700; 
                                              font-size: 17px;
                                              box-shadow: 0 6px 16px rgba(251, 191, 36, 0.4);
                                              transition: all 0.3s ease;
                                              text-transform: uppercase;
                                              letter-spacing: 0.8px;'>
                                        ✉️ Verify Email Address
                                    </a>
                                </div>
                                
                                <!-- Alternative Link -->
                                <div style='background-color: #fefce8; padding: 22px; border-radius: 10px; margin: 30px 0; border: 1px solid #fde68a;'>
                                    <p style='color: #6b7280; margin: 0 0 12px 0; font-size: 15px;'>
                                        <strong>Button not working?</strong> Copy and paste this link into your browser:
                                    </p>
                                    <p style='word-break: break-all; color: #d97706; font-size: 14px; margin: 0; font-family: monospace; background: white; padding: 10px; border-radius: 6px; border: 1px solid #fde68a;'>
                                        {verificationUrl}
                                    </p>
                                </div>
                                
                                <!-- Important Info -->
                                <div style='border: 2px solid #fde68a; border-radius: 10px; padding: 18px; margin: 30px 0; background: #fffbeb;'>
                                    <p style='margin: 0; color: #92400e; font-size: 15px;'>
                                        ⏰ <strong>Important:</strong> This verification link will expire in <strong>24 hours</strong> for security reasons.
                                    </p>
                                </div>
                                
                                <p style='color: #6b7280; line-height: 1.6; margin: 25px 0 0 0; font-size: 15px;'>
                                    If you didn't create an account with TaskHive, please ignore this email and no action is required.
                                </p>
                            </div>
                            
                            <!-- Footer -->
                            <div style='background-color: #fefce8; padding: 35px; text-align: center; border-top: 1px solid #fde68a;'>
                                <p style='color: #6b7280; font-size: 15px; margin: 0 0 12px 0;'>
                                    Best regards,<br>
                                    <strong style='color: #d97706;'>The TaskHive Team</strong> 🐝
                                </p>
                                <p style='color: #9ca3af; font-size: 13px; margin: 0;'>
                                    © 2024 TaskHive. Building the future of work, one project at a time.
                                </p>
                            </div>
                        </div>
                    </body>
                    </html>";

                return await SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending verification email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string email, string fullName)
        {
            try
            {
                var subject = "🎉 Welcome to TaskHive - Your Account is Verified!";
                var body = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Welcome to TaskHive</title>
                    </head>
                    <body style='margin: 0; padding: 0; background-color: #f0fdf4;'>
                        <div style='font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif; max-width: 600px; margin: 0 auto; background-color: #ffffff; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);'>
                            
                            <!-- Success Header -->
                            <div style='background: linear-gradient(135deg, #10b981 0%, #059669 50%, #047857 100%); color: white; padding: 50px 20px; text-align: center;'>
                                <!-- Success Icon -->
                                <div style='margin-bottom: 25px;'>
                                    <div style='display: inline-block; width: 90px; height: 90px; background: rgba(255,255,255,0.15); border-radius: 50%; position: relative; border: 3px solid rgba(255,255,255,0.3);'>
                                        <div style='position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); font-size: 45px;'>
                                            ✅
                                        </div>
                                    </div>
                                </div>
                                
                                <h1 style='margin: 0; font-size: 34px; font-weight: 700;'>
                                    Account Verified Successfully!
                                </h1>
                                <p style='margin: 12px 0 0 0; font-size: 19px; opacity: 0.95; font-weight: 300;'>
                                    You're all set to start your journey 🚀
                                </p>
                            </div>
                            
                            <!-- Content -->
                            <div style='padding: 45px 35px;'>
                                <h2 style='color: #d97706; font-size: 26px; margin: 0 0 22px 0;'>
                                    Congratulations, {fullName}! 🎊
                                </h2>
                                
                                <p style='color: #374151; line-height: 1.7; margin: 0 0 28px 0; font-size: 17px;'>
                                    Your <strong style='color: #d97706;'>TaskHive</strong> account has been successfully verified! You now have access to all features of our platform.
                                </p>
                                
                                <!-- Features List -->
                                <div style='background: linear-gradient(135deg, #fef3c7, #fde68a); padding: 28px; border-radius: 15px; margin: 30px 0;'>
                                    <h3 style='color: #92400e; margin: 0 0 18px 0; font-size: 19px;'>
                                        🚀 What you can do now:
                                    </h3>
                                    <ul style='color: #374151; line-height: 1.9; margin: 0; padding-left: 22px; font-size: 16px;'>
                                        <li><strong>Browse and apply</strong> for exciting projects</li>
                                        <li><strong>Create and showcase</strong> your professional profile</li>
                                        <li><strong>Connect and collaborate</strong> with clients worldwide</li>
                                        <li><strong>Take skill assessments</strong> to boost your credibility</li>
                                        <li><strong>Manage projects</strong> and track your progress</li>
                                    </ul>
                                </div>
                                
                                <!-- CTA Button -->
                                <div style='text-align: center; margin: 40px 0;'>
                                    <a href='{_configuration["Frontend:BaseUrl"] ?? "http://localhost:3000"}/dashboard' 
                                       style='background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 50%, #d97706 100%); 
                                              color: white; 
                                              padding: 18px 45px; 
                                              text-decoration: none; 
                                              border-radius: 15px; 
                                              display: inline-block; 
                                              font-weight: 700; 
                                              font-size: 17px;
                                              box-shadow: 0 6px 16px rgba(251, 191, 36, 0.4);
                                              text-transform: uppercase;
                                              letter-spacing: 0.8px;'>
                                        🏠 Go to Dashboard
                                    </a>
                                </div>
                                
                                <div style='background-color: #f0fdf4; border-left: 5px solid #10b981; padding: 22px; border-radius: 10px; margin: 30px 0;'>
                                    <p style='margin: 0; color: #166534; font-size: 15px;'>
                                        💡 <strong>Pro Tip:</strong> Complete your profile to attract more clients and increase your chances of landing great projects!
                                    </p>
                                </div>
                                
                                <p style='color: #374151; line-height: 1.7; margin: 30px 0 0 0; font-size: 17px;'>
                                    Thank you for joining <strong style='color: #d97706;'>TaskHive</strong>. We're excited to have you on board and can't wait to see what amazing projects you'll work on! 🌟
                                </p>
                            </div>
                            
                            <!-- Footer -->
                            <div style='background-color: #fefce8; padding: 35px; text-align: center; border-top: 1px solid #fde68a;'>
                                <p style='color: #6b7280; font-size: 15px; margin: 0 0 12px 0;'>
                                    Best regards,<br>
                                    <strong style='color: #d97706;'>The TaskHive Team</strong> 🐝
                                </p>
                                <p style='color: #9ca3af; font-size: 13px; margin: 0;'>
                                    © 2024 TaskHive. Building the future of work, one project at a time.
                                </p>
                            </div>
                        </div>
                    </body>
                    </html>";

                return await SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending welcome email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string fullName, string resetToken)
        {
            try
            {
                var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";
                var resetUrl = $"{frontendUrl}/reset-password?token={resetToken}";

                var subject = "🔐 Reset Your TaskHive Password";
                var body = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Reset Your Password</title>
                    </head>
                    <body style='margin: 0; padding: 0; background-color: #fef2f2;'>
                        <div style='font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif; max-width: 600px; margin: 0 auto; background-color: #ffffff; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);'>
                            
                            <!-- Header -->
                            <div style='background: linear-gradient(135deg, #ef4444 0%, #dc2626 50%, #b91c1c 100%); color: white; padding: 50px 20px; text-align: center;'>
                                <div style='margin-bottom: 25px;'>
                                    <div style='display: inline-block; width: 90px; height: 90px; background: rgba(255,255,255,0.15); border-radius: 50%; position: relative; border: 3px solid rgba(255,255,255,0.3);'>
                                        <div style='position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); font-size: 45px;'>
                                            🔐
                                        </div>
                                    </div>
                                </div>
                                
                                <h1 style='margin: 0; font-size: 34px; font-weight: 700;'>
                                    Password Reset Request
                                </h1>
                                <p style='margin: 12px 0 0 0; font-size: 19px; opacity: 0.95; font-weight: 300;'>
                                    Secure your TaskHive account 🔒
                                </p>
                            </div>
                            
                            <!-- Content -->
                            <div style='padding: 45px 35px;'>
                                <h2 style='color: #d97706; font-size: 26px; margin: 0 0 22px 0;'>
                                    Hi {fullName}! 👋
                                </h2>
                                
                                <p style='color: #374151; line-height: 1.7; margin: 0 0 28px 0; font-size: 17px;'>
                                    We received a request to reset your <strong style='color: #d97706;'>TaskHive</strong> account password. Click the button below to create a new password.
                                </p>
                                
                                <!-- CTA Button -->
                                <div style='text-align: center; margin: 40px 0;'>
                                    <a href='{resetUrl}' 
                                       style='background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 50%, #d97706 100%); 
                                              color: white; 
                                              padding: 18px 45px; 
                                              text-decoration: none; 
                                              border-radius: 15px; 
                                              display: inline-block; 
                                              font-weight: 700; 
                                              font-size: 17px;
                                              box-shadow: 0 6px 16px rgba(251, 191, 36, 0.4);
                                              text-transform: uppercase;
                                              letter-spacing: 0.8px;'>
                                        🔑 Reset Password
                                    </a>
                                </div>
                                
                                <div style='border: 2px solid #fde68a; border-radius: 10px; padding: 18px; margin: 30px 0; background: #fffbeb;'>
                                    <p style='margin: 0; color: #92400e; font-size: 15px;'>
                                        ⏰ <strong>Important:</strong> This reset link will expire in <strong>1 hour</strong> for security reasons.
                                    </p>
                                </div>
                                
                                <p style='color: #6b7280; line-height: 1.6; margin: 25px 0 0 0; font-size: 15px;'>
                                    If you didn't request a password reset, please ignore this email and your password will remain unchanged.
                                </p>
                            </div>
                            
                            <!-- Footer -->
                            <div style='background-color: #fefce8; padding: 35px; text-align: center; border-top: 1px solid #fde68a;'>
                                <p style='color: #6b7280; font-size: 15px; margin: 0 0 12px 0;'>
                                    Best regards,<br>
                                    <strong style='color: #d97706;'>The TaskHive Team</strong> 🐝
                                </p>
                            </div>
                        </div>
                    </body>
                    </html>";

                return await SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending password reset email: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["Email:Username"];
                var smtpPassword = _configuration["Email:Password"];
                var fromEmail = _configuration["Email:FromEmail"];
                var fromName = _configuration["Email:FromName"] ?? "TaskHive";

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail!, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SMTP Error: {ex.Message}");
                return false;
            }
        }
    }
}