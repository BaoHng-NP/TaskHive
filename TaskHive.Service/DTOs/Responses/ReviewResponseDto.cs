using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Service.DTOs.Responses.User;

namespace TaskHive.Service.DTOs.Responses
{
    public class ReviewResponseDto
    {
        public int ReviewId { get; set; }

        public int ReviewerId { get; set; }

        public UserResponseDto Reviewer { get; set; } = null!;

        public int RevieweeId { get; set; }

        public UserResponseDto Reviewee { get; set; } = null!;

        public int JobPostId { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
