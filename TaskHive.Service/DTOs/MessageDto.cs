using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Enums;

namespace TaskHive.Service.DTOs
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = null!;
        public string? FileURL { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
