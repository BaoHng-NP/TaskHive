using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Service.DTOs.Requests.SlotPurchase
{
    public class UpdateSlotPurchaseRequestDto
    {
        [Required]
        public int SlotPurchaseId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        public bool? IsActive { get; set; }
    }
}
