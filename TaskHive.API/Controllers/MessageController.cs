using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using TaskHive.API.Extensions;
using TaskHive.API.Hubs;
using TaskHive.Repository.Enums;
using TaskHive.Service.DTOs;
using TaskHive.Service.Services.ChatService;
using TaskHive.Service.Services.MessageService;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _svc;
        private readonly IChatService _chat;
        private readonly IHubContext<ChatHub> _hub;

        public MessageController(IMessageService svc, IChatService chat, IHubContext<ChatHub> hub)
        {
            _svc = svc;
            _chat = chat;
            _hub = hub;
        }

        [HttpGet("{conversationId}")]
        [ProducesResponseType(typeof(IEnumerable<MessageDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetByConversation(int conversationId)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            if (!await _chat.IsMemberAsync(conversationId, userId.Value)) return Forbid();

            var msgs = await _svc.GetByConversationIdAsync(conversationId);
            return Ok(msgs);
        }

        [HttpPost("{conversationId}")]
        [ProducesResponseType(typeof(MessageDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Send(int conversationId, SendMessageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            if (!await _chat.IsMemberAsync(conversationId, userId.Value)) return Forbid();

            dto.SenderId = userId.Value; // người gửi luôn là người đang đăng nhập
            dto.Content = dto.Content?.Trim() ?? string.Empty;
            if (dto.Content.Length == 0 && string.IsNullOrEmpty(dto.FileURL))
                return BadRequest(new { message = "Empty message" });
            dto.MessageType = string.IsNullOrEmpty(dto.FileURL) ? MessageType.Text : MessageType.File;

            var msg = await _svc.SendMessageAsync(conversationId, dto);
            await _hub.Clients.Group(conversationId.ToString())
                          .SendAsync("ReceiveMessage", msg);
            return CreatedAtAction(nameof(GetByConversation), new { conversationId }, msg);
        }
    }
}
