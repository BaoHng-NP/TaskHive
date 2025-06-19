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

        public int MonthlySlotLimit { get; set; }

        public string? Features { get; set; }

        // Có thể thêm nếu muốn cho phép client set
        public bool Status { get; set; } = true;
    }
}
