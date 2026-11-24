using System.Security.Claims;
using TaskHive.Repository.Enums;

namespace TaskHive.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>UserId của người đang gọi API, lấy từ JWT (không bao giờ lấy từ body/query).</summary>
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(value, out var id) ? id : null;
        }

        public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole(nameof(UserRole.Admin));

        /// <summary>True nếu người gọi chính là <paramref name="userId"/> hoặc là Admin.</summary>
        public static bool IsSelfOrAdmin(this ClaimsPrincipal user, int userId)
            => user.IsAdmin() || user.GetUserId() == userId;
    }
}
