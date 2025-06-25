using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.ConversationRepository
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly AppDbContext _db;
        public ConversationRepository(AppDbContext db) => _db = db;

        public async Task<Conversation?> GetByIdAsync(int conversationId)
        {
            return await _db.Conversations
                .Include(c => c.Members)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);
        }

        public async Task<Conversation> CreateAsync(Conversation conversation)
        {
            _db.Conversations.Add(conversation);
            await _db.SaveChangesAsync();
            return conversation;
        }

        public async Task AddMemberAsync(ConversationMember member)
        {
            _db.ConversationMembers.Add(member);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ConversationMember>> GetMembersAsync(int conversationId)
        {
            return await _db.ConversationMembers
                .Where(cm => cm.ConversationId == conversationId)
                .Include(cm => cm.User)
                .ToListAsync();
        }
    }
}
