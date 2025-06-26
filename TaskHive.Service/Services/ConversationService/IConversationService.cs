using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Service.DTOs;

namespace TaskHive.Service.Services.ConversationService
{
    public interface IConversationService
    {
        Task<ConversationDto?> GetByIdAsync(int conversationId);
        Task<ConversationDto> CreateAsync(CreateConversationDto dto);
        Task AddMemberAsync(int conversationId, int memberId);
    }
}
