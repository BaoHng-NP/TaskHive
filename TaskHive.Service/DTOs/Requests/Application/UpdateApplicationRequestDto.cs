using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TaskHive.Service.DTOs.Requests.Application
{
    public class UpdateApplicationRequestDto
    {
        [Required]
        public int ApplicationId { get; set; }
        public string? CoverLetter { get; set; }
        public decimal? BidAmount { get; set; }
        public string? Status { get; set; }
        public IFormFile? CVFile { get; set; }
    }
}