using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Service.DTOs
{
    public class CreateConversationDto
    {
        public string Type { get; set; } = null!;
        public int CreatedBy { get; set; }
    }
}
