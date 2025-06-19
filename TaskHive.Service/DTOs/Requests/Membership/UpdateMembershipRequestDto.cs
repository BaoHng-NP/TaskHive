using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Service.DTOs.Requests.Membership
{
    public class UpdateMembershipRequestDto
    {
        [Required]
        public int MembershipId { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
    }
}
