using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using TaskHive.Service.DTOs;
using TaskHive.Service.Services.MessageService;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _svc;

        public MessageController(IMessageService svc)
        {
            _svc = svc;
        }

        [HttpGet("{conversationId}")]
        [ProducesResponseType(typeof(IEnumerable<MessageDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetByConversation(int conversationId)
        {
            var msgs = await _svc.GetByConversationIdAsync(conversationId);
            return Ok(msgs);
        }

        [HttpPost("{conversationId}")]
        [ProducesResponseType(typeof(MessageDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Send(int conversationId, SendMessageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var msg = await _svc.SendMessageAsync(conversationId, dto);
            return CreatedAtAction(nameof(GetByConversation), new { conversationId }, msg);
        }
    }
}