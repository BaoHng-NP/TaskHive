using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Service.DTOs.Requests.UserMembership
{
    public class UpdateUserMembershipRequestDto
    {
        [Required]
        public int UserMembershipId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? RemainingSlots { get; set; }

        public bool? IsActive { get; set; }
    }
}
