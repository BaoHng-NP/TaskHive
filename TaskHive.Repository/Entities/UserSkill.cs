using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Repository.Entities
{
    public class UserSkill
    {
        [Key, Column(Order = 0)]
        public int CategoryId { get; set; }

        [Key, Column(Order = 1)]
        public int UserId { get; set; }

        public Category Category { get; set; } = null!;
        public Freelancer User { get; set; } = null!;
    }
}
