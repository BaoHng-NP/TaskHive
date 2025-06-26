using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TaskHive.Repository.Enums;
using TaskHive.Service.DTOs;
using TaskHive.Service.Services.ChatService;

namespace TaskHive.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        // Khi client kết nối với ?conversationId=123
        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            if (!int.TryParse(http.Request.Query["conversationId"], out var convId))
                throw new HubException("Invalid conversationId");

            // Validate that this user is member
            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (!await _chatService.IsMemberAsync(convId, userId))
                throw new HubException("You are not a member of this conversation");


            await Groups.AddToGroupAsync(Context.ConnectionId, convId.ToString());
            _logger.LogInformation("Connection {ConnectionId} joined group {Group}", Context.ConnectionId, convId);
            await base.OnConnectedAsync();
        }

        // Khi client ngắt kết nối
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var http = Context.GetHttpContext();
            if (int.TryParse(http.Request.Query["conversationId"], out var convId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, convId.ToString());
                _logger.LogInformation("Connection {ConnectionId} left group {Group}", Context.ConnectionId, convId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<(IEnumerable<MessageDto> Messages, bool HasMore)> LoadHistory(
        int conversationId, int page = 1, int pageSize = 50)
        {
            var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (!await _chatService.IsMemberAsync(conversationId, userId))
                throw new HubException("Not authorized");

            return await _chatService.GetMessagesPagedAsync(conversationId, page, pageSize);
        }

        // Gửi tin
        public async Task SendMessage(int conversationId, string content, string? fileUrl)
        {
            try
            {
                // Xác thực
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var senderId))
                    throw new HubException("User not authenticated");

                // Chuẩn bị DTO
                var dto = new SendMessageDto
                {
                    SenderId = senderId,
                    Content = content?.Trim() ?? "",
                    FileURL = fileUrl,
                    MessageType = string.IsNullOrEmpty(fileUrl)
                                  ? MessageType.Text
                                  : MessageType.File
                };

                // validate length
                if (dto.Content.Length == 0 && dto.FileURL == null)
                    throw new HubException("Empty message");

                // Lưu và trả về DTO
                var msgDto = await _chatService.SendMessageAsync(conversationId, dto);

                // Broadcast về nhóm
                await Clients.Group(conversationId.ToString())
                             .SendAsync("ReceiveMessage", msgDto);
            }
            catch (HubException ex)
            {
                _logger.LogWarning(ex, "Error sending message to conversation {ConvId}", conversationId);
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }
    }
}
