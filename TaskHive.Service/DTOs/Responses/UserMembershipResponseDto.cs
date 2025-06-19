using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Service.DTOs.Responses
{
    public class UserMembershipResponseDto
    {
        public int UserMembershipId { get; set; }
        public int UserId { get; set; }
        public int MembershipId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RemainingSlots { get; set; }
        public bool IsActive { get; set; }
    }
}
