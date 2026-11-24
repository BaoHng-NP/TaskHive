// ConversationController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskHive.API.Extensions;
using TaskHive.Service.DTOs;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.Services.ChatService;
using TaskHive.Service.Services.ConversationService;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _svc;
        private readonly IChatService _chat;

        public ConversationController(IConversationService svc, IChatService chat)
        {
            _svc = svc;
            _chat = chat;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ConversationDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var conv = await _svc.GetByIdAsync(id);
            if (conv == null) return NotFound();
            if (!User.IsAdmin() && !conv.MemberIds.Contains(User.GetUserId() ?? 0)) return Forbid();
            return Ok(conv);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ConversationDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(CreateConversationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            dto.CreatedBy = userId.Value; // người tạo luôn là người đang đăng nhập

            var conv = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = conv.ConversationId }, conv);
        }

        /// <summary>Chỉ thành viên hiện tại của hội thoại (hoặc Admin) được thêm người khác vào.</summary>
        [HttpPost("{id}/members/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> AddMember(int id, int userId)
        {
            var me = User.GetUserId();
            if (me == null) return Unauthorized();
            if (!User.IsAdmin() && !await _chat.IsMemberAsync(id, me.Value)) return Forbid();
            if (await _chat.IsMemberAsync(id, userId)) return NoContent(); // đã là thành viên

            await _svc.AddMemberAsync(id, userId);
            return NoContent();
        }

        /// <summary>
        /// Lấy tất cả hội thoại mà freelancer (userId) tham gia.
        /// Trả về partner là phía Client (tên + avatar) để hiển thị danh sách.
        /// </summary>
        [HttpGet("freelancer/{userId:int}")]
        [ProducesResponseType(typeof(IEnumerable<ConversationListItemDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetForFreelancer(int userId)
        {
            if (!User.IsSelfOrAdmin(userId)) return Forbid();
            var list = await _svc.GetForFreelancerAsync(userId);
            return Ok(list);
        }

        /// <summary>
        /// Lấy tất cả hội thoại mà client (userId) tham gia.
        /// Trả về partner là phía Freelancer (tên + avatar) để hiển thị danh sách.
        /// </summary>
        [HttpGet("client/{userId:int}")]
        [ProducesResponseType(typeof(IEnumerable<ConversationListItemDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetForClient(int userId)
        {
            if (!User.IsSelfOrAdmin(userId)) return Forbid();
            var list = await _svc.GetForClientAsync(userId);
            return Ok(list);
        }
    }
}
