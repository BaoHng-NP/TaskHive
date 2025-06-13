using System.ComponentModel.DataAnnotations;

namespace TaskHive.Service.DTOs.Requests.Category
{
    public class AddCategoryRequestDto
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}