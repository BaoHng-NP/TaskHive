using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Service.DTOs;

namespace TaskHive.Service.Services.ChatService
{
    public interface IChatService
    {
        Task<ConversationDto> CreateConversationAsync(CreateConversationDto dto);
        Task AddMemberAsync(int conversationId, int userId);
        Task<MessageDto> SendMessageAsync(int conversationId, SendMessageDto dto);
        Task<IEnumerable<MessageDto>> GetMessagesAsync(int conversationId);
        Task<bool> IsMemberAsync(int conversationId, int userId);
        Task<(IEnumerable<MessageDto> Messages, bool HasMore)> GetMessagesPagedAsync(
            int conversationId, int page, int pageSize);
    }
}
