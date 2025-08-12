using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Threading.Tasks;
using TaskHive.API.Hubs;
using TaskHive.Service.DTOs;
using TaskHive.Service.Services.MessageService;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _svc;
        private readonly IHubContext<ChatHub> _hub;

        public MessageController(IMessageService svc, IHubContext<ChatHub> hub)
        {
            _svc = svc;
            _hub = hub;
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
            await _hub.Clients.Group(conversationId.ToString())
                          .SendAsync("ReceiveMessage", msg);
            return CreatedAtAction(nameof(GetByConversation), new { conversationId }, msg);
        }
    }
}