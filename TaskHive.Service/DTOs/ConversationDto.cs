using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Service.DTOs
{
    public class ConversationDto
    {
        public int ConversationId { get; set; }
        public string Type { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<int> MemberIds { get; set; } = new List<int>();
    }
}
