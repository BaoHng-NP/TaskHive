using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Service.DTOs.Responses.User
{
    public class FreelancerListItemWithRatingResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Country { get; set; }
        public string? ImageUrl { get; set; }

        public int ReviewCount { get; set; }
        public decimal AverageRating { get; set; }

        // tái dùng UserSkillDto để FE nhận { categoryId, categoryName, ... }
        public List<UserSkillDto> Skills { get; set; } = new();

        // tuỳ bạn có trường mô tả hay không
        public string? About { get; set; }
    }
}
