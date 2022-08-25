using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using ReenbitChat.Database;
using ReenbitChat.Hubs;
using ReenbitChat.Models;

namespace ReenbitChat.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ChatController : Controller
    {
        private IHubContext<ChatHub> _chat;
        private AppDbContext _context;

        public ChatController(IHubContext<ChatHub> chat, AppDbContext context)
        {
            _chat = chat;
            _context = context;
        }

        [HttpPost("[action]/{connectionId}/{chatName}")]
        public async Task<IActionResult> JoinChat(string connectionId, string chatName)
        {
            await _chat.Groups.AddToGroupAsync(connectionId, chatName);
            return Ok();
        }

        [HttpPost("[action]/{connectionId}/{chatName}")]
        public async Task<IActionResult> LeaveChat(string connectionId, string chatName)
        {
            await _chat.Groups.RemoveFromGroupAsync(connectionId, chatName);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(string message, int chatId,
            string chatName, [FromServices] AppDbContext context)
        {
            var Message = new Message
            {
                ChatId = chatId,
                Content = message,
                SenderName = User.Identity.Name,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(Message);
            await _context.SaveChangesAsync();

            await _chat.Clients.Group(chatName)
                .SendAsync("RecieveMessage", Message);

            return Ok();
        }

        [HttpPut("[action]/{connectionId}/{messageId}")]
        public async Task<IActionResult> EditMessage(string message, int chatId,
            int messageId, string chatName, [FromServices] AppDbContext context)
        {
            var Message = context.Messages
                .SingleOrDefault(x => x.Id == messageId);

            _context.Messages.Update(Message);
            await _context.SaveChangesAsync();

            await _chat.Clients.Group(chatName).SendAsync("RecieveMessage", Message);

            return Ok();
        }

        [HttpDelete("[action]/{connectionId}/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int chatId, int messageId,
            string userId, [FromServices] AppDbContext context)
        {
            var Message = context.Messages
                .SingleOrDefault(x => x.Id == messageId);

            _context.Messages.Remove(Message);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
