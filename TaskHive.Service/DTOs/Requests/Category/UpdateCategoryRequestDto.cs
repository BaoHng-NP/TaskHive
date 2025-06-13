using System.ComponentModel.DataAnnotations;

namespace TaskHive.Service.DTOs.Requests.Category
{
    public class UpdateCategoryRequestDto
    {
        [Required]
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}