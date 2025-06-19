using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Service.DTOs.Requests.UserMembership
{
    public class AddUserMembershipRequestDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int MembershipId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int RemainingSlots { get; set; }

        public bool IsActive { get; set; }
    }
}
