using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Repository.Entities
{
    public class ConversationMember
    {
        [Key, Column(Order = 0)]
        public int ConversationId { get; set; }

        [ForeignKey("ConversationId")]
        public Conversation Conversation { get; set; } = null!;

        [Key, Column(Order = 1)]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required]
        public DateTime JoinedAt { get; set; }

        public int? LastReadMessageId { get; set; }

        [ForeignKey("LastReadMessageId")]
        public Message? LastReadMessage { get; set; }
    }
}
