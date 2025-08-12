using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Service.DTOs.Responses
{
    public class ConversationListItemDto
    {
        public int ConversationId { get; set; }

        // Partner (người đối thoại với userId hiện tại)
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = "";
        public string? PartnerAvatarUrl { get; set; }

        // Hiển thị preview trong list
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }

        // Tùy CSDL có trường IsRead/ReceiverId hay không; nếu chưa có thì tạm = 0
        public int UnreadCount { get; set; }
    }
}
