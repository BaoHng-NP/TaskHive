using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Repository.Repositories.ConversationRepository;
using TaskHive.Repository.Repositories.MessageRepository;
using TaskHive.Service.DTOs;

namespace TaskHive.Service.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IConversationRepository _convRepo;
        private readonly IMessageRepository _msgRepo;
        private readonly IMapper _mapper;

        public ChatService(
            IConversationRepository convRepo,
            IMessageRepository msgRepo,
            IMapper mapper)
        {
            _convRepo = convRepo;
            _msgRepo = msgRepo;
            _mapper = mapper;
        }

        public async Task<ConversationDto> CreateConversationAsync(CreateConversationDto dto)
        {
            var conv = _mapper.Map<Conversation>(dto);
            conv.CreatedAt = DateTime.UtcNow;
            var created = await _convRepo.CreateAsync(conv);
            return _mapper.Map<ConversationDto>(created);
        }

        public async Task AddMemberAsync(int conversationId, int userId)
        {
            var member = new ConversationMember
            {
                ConversationId = conversationId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow,
            };
            await _convRepo.AddMemberAsync(member);
        }

        public async Task<MessageDto> SendMessageAsync(int conversationId, SendMessageDto dto)
        {
            var msg = _mapper.Map<Message>(dto);
            msg.ConversationId = conversationId;
            msg.CreatedAt = DateTime.UtcNow;
            // senderId should be set by Hub via dto or context
            var saved = await _msgRepo.AddAsync(msg);
            return _mapper.Map<MessageDto>(saved);
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesAsync(int conversationId)
        {
            var list = await _msgRepo.GetByConversationIdAsync(conversationId);
            return _mapper.Map<IEnumerable<MessageDto>>(list);
        }

        public async Task<bool> IsMemberAsync(int conversationId, int userId)
        {
            var members = await _convRepo.GetMembersAsync(conversationId);
            return members.Any(m => m.UserId == userId);
        }

        public async Task<(IEnumerable<MessageDto>, bool)> GetMessagesPagedAsync(
            int conversationId, int page, int pageSize)
        {
            var all = await _msgRepo.GetByConversationIdAsync(conversationId);
            var paged = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtos = _mapper.Map<IEnumerable<MessageDto>>(paged);
            bool hasMore = all.Count() > page * pageSize;
            return (dtos, hasMore);
        }
    }
}
