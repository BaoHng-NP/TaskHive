using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.MessageRepository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _db;
        public MessageRepository(AppDbContext db) => _db = db;

        public async Task<Message> AddAsync(Message message)
        {
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
            return message;
        }

        public async Task<IEnumerable<Message>> GetByConversationIdAsync(int conversationId)
        {
            return await _db.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}
