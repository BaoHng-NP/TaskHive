using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.DTOs;

namespace TaskHive.Service.Services.MessageService
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public MessageService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MessageDto>> GetByConversationIdAsync(int conversationId)
        {
            var msgs = await _uow.Messages.GetByConversationIdAsync(conversationId);
            return _mapper.Map<IEnumerable<MessageDto>>(msgs);
        }

        public async Task<MessageDto> SendMessageAsync(int conversationId, SendMessageDto dto)
        {
            var entity = new Message
            {
                ConversationId = conversationId,
                SenderId = dto.SenderId,
                Content = dto.Content,
                FileURL = dto.FileURL,
                MessageType = dto.MessageType,
                CreatedAt = DateTime.UtcNow
            };
            var added = await _uow.Messages.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return _mapper.Map<MessageDto>(added);
        }
    }
}
