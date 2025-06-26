using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Enums;

namespace TaskHive.Service.DTOs
{
    public class SendMessageDto
    {
        public int SenderId { get; set; }
        public string Content { get; set; } = null!;
        public string? FileURL { get; set; }
        public MessageType MessageType { get; set; }
    }
}
