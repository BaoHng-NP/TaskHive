using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Repository.Enums;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.DTOs;

namespace TaskHive.Service.Services.ConversationService
{
    public class ConversationService : IConversationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ConversationService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ConversationDto?> GetByIdAsync(int conversationId)
        {
            var conv = await _uow.Conversations.GetByIdAsync(conversationId);
            if (conv == null) return null;

            var dto = _mapper.Map<ConversationDto>(conv);
            dto.MemberIds = conv.Members.Select(m => m.UserId);
            return dto;
        }

        public async Task<ConversationDto> CreateAsync(CreateConversationDto dto)
        {
            var entity = new Conversation
            {
                Type = Enum.Parse<ConversationType>(dto.Type, true),
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };
            // tạo conversation
            var created = await _uow.Conversations.CreateAsync(entity);
            // tự động thêm creator làm member
            await _uow.Conversations.AddMemberAsync(new ConversationMember
            {
                ConversationId = created.ConversationId,
                UserId = dto.CreatedBy
            });

            await _uow.SaveChangesAsync();

            var result = _mapper.Map<ConversationDto>(created);
            result.MemberIds = new[] { dto.CreatedBy };
            return result;
        }

        public async Task AddMemberAsync(int conversationId, int memberId)
        {
            await _uow.Conversations.AddMemberAsync(new ConversationMember
            {
                ConversationId = conversationId,
                UserId = memberId
            });
            await _uow.SaveChangesAsync();
        }
    }
}
