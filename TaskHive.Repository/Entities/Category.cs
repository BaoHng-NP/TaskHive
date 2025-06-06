using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Repository.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
        public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();

    }
}
