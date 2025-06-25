using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Service.DTOs;

namespace TaskHive.Service.Services.MessageService
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDto>> GetByConversationIdAsync(int conversationId);
        Task<MessageDto> SendMessageAsync(int conversationId, SendMessageDto dto);
    }
}
