using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReenbitChat.Database;
using ReenbitChat.Models;
using System.Security.Claims;

namespace ReenbitChat.ViewComponents
{
    public class ChatViewComponent : ViewComponent
    {
        private AppDbContext _ctx;

        public ChatViewComponent(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public IViewComponentResult Invoke()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var chats = _ctx.ChatUsers
                .Include(x => x.Chat)
                .Where(x => x.UserId == userId
                    && x.Chat.Type == ChatType.Group)
                .Select(x => x.Chat)
                .ToList();

            return View(chats);
        }
    }
}
