// ConversationController.cs
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using TaskHive.Service.DTOs;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.Services.ConversationService;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _svc;

        public ConversationController(IConversationService svc)
        {
            _svc = svc;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ConversationDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var conv = await _svc.GetByIdAsync(id);
            return conv == null ? NotFound() : Ok(conv);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ConversationDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(CreateConversationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var conv = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = conv.ConversationId }, conv);
        }

        [HttpPost("{id}/members/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> AddMember(int id, int userId)
        {
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
            var list = await _svc.GetForClientAsync(userId);
            return Ok(list);
        }
    }
}
