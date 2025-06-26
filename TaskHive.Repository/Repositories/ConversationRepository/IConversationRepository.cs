using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.ConversationRepository
{
    public interface IConversationRepository
    {
        Task<Conversation?> GetByIdAsync(int conversationId);
        Task<Conversation> CreateAsync(Conversation conversation);
        Task AddMemberAsync(ConversationMember member);
        Task<IEnumerable<ConversationMember>> GetMembersAsync(int conversationId);
    }
}
