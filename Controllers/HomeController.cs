using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ReenbitChat.Database;
using ReenbitChat.Hubs;
using ReenbitChat.Infrastructure;
using ReenbitChat.Infrastructure.Respository;
using ReenbitChat.Models;

namespace ReenbitChat.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private IChatRepository _repo;
        public HomeController(IChatRepository repo) => _repo = repo;
        public IActionResult Index()
        {
            var chats = _repo.GetChats(GetUserId());

            return View(chats);
        }

        public IActionResult Find([FromServices] AppDbContext context)
        {
            var users = context.Users
                .Where(x => x.Id != User.GetUserId())
                .ToList();

            return View(users);
        }

        public IActionResult Private()
        {
            var chats = _repo.GetPrivateChats(GetUserId());

            return View(chats);
        }

        public async Task<IActionResult> CreatePrivateChat(string userId)
        {
            var id = await _repo.CreatePrivateChat(GetUserId(), userId);

            return RedirectToAction("Chat", new { id });
        }

        [HttpGet("{id}")]
        public IActionResult Chat(int id)
        {
            return View(_repo.GetChat(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat(string name)
        {
            await _repo.CreateChat(name, GetUserId());
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> JoinChat(int id)
        {
            await _repo.JoinChat(id, GetUserId());

            return RedirectToAction("Chat", "Home", new { id = id });
        }

        public async Task<IActionResult> SendMessage(
            int chatId,
            string message,
            [FromServices] IHubContext<ChatHub> chat)
        {
            var Message = await _repo.CreateMessage(chatId, message, User.Identity.Name);

            await chat.Clients.Group(chatId.ToString())
                .SendAsync("RecieveMessage", new
                {
                    Text = Message.Content,
                    Name = Message.SenderName,
                    Timestamp = Message.Timestamp.ToString("dd/MM/yyyy hh:mm:ss")
                });

            Redirect(chatId.ToString());
            return Ok();
        }
    }
}
