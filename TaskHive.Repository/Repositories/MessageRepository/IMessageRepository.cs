using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.MessageRepository
{
    public interface IMessageRepository
    {
        Task<Message> AddAsync(Message message);
        Task<IEnumerable<Message>> GetByConversationIdAsync(int conversationId);
    }
}
