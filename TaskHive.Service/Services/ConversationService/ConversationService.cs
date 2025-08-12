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
using TaskHive.Service.DTOs.Responses;

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

        public async Task<IEnumerable<ConversationListItemDto>> GetForFreelancerAsync(int userId)
            => await BuildList(userId);

        public async Task<IEnumerable<ConversationListItemDto>> GetForClientAsync(int userId)
            => await BuildList(userId);

        public async Task<IEnumerable<ConversationListItemDto>> BuildList(int userId)
        {
            var conversations = await _uow.Conversations.GetForUserAsync(userId);

            var list = conversations.Select(c =>
            {
                // Chỉ lấy 1-1: partner là người khác userId
                var partnerMember = c.Members.FirstOrDefault(m => m.UserId != userId);
                var partnerUser = partnerMember?.User;

                var last = c.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefault();

                // Nếu DB chưa có IsRead/ReceiverId, UnreadCount = 0
                var unread = 0;
                try
                {
                    unread = c.Messages.Count(m =>
                        (bool?)m.GetType().GetProperty("IsRead")?.GetValue(m) == false &&
                        (int?)m.GetType().GetProperty("ReceiverId")?.GetValue(m) == userId
                    );
                }
                catch { /* ignore if fields not exist */ }

                return new ConversationListItemDto
                {
                    ConversationId = c.ConversationId,
                    PartnerId = partnerUser?.UserId ?? 0,
                    PartnerName = partnerUser?.FullName
                        ?? partnerUser?.UserName
                        ?? "Unknown",
                    PartnerAvatarUrl = partnerUser?.imageUrl,
                    LastMessage = last?.Content,
                    LastMessageAt = last?.CreatedAt,
                    UnreadCount = unread
                };
            })
            // Sắp xếp theo last message desc (đẩy null xuống cuối)
            .OrderByDescending(i => i.LastMessageAt ?? DateTime.MinValue)
            .ToList();

            return list;
        }
    }
}
