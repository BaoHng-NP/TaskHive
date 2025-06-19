using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Service.DTOs.Requests.Membership
{
    public class AddMembershipRequestDto
    {
        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }
    }
}
