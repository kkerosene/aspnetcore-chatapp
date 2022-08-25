using Microsoft.EntityFrameworkCore;
using ReenbitChat.Database;
using ReenbitChat.Models;

namespace ReenbitChat.Infrastructure.Respository
{
    public class ChatRepository : IChatRepository
    {
        private AppDbContext _context;

        public ChatRepository(AppDbContext context) => _context = context;

        public async Task<Message> CreateMessage(int chatId, string message, string userId)
        {
            var Message = new Message
            {
                ChatId = chatId,
                Content = message,
                SenderName = userId,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(Message);
            await _context.SaveChangesAsync();

            return Message;
        }

        public async Task<int> CreatePrivateChat(string rootId, string targetId)
        {
            var chat = new Chat
            {
                Type = ChatType.Private
            };

            chat.Users.Add(new ChatUser
            {
                UserId = targetId
            });

            chat.Users.Add(new ChatUser
            {
                UserId = rootId
            });

            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();

            return chat.ChatId;
        }

        public async Task CreateChat(string name, string userId)
        {
            var chat = new Chat
            {
                Name = name,
                Type = ChatType.Group
            };

            chat.Users.Add(new ChatUser
            {
                UserId = userId,
                Role = UserRole.Admin
            });

            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();
        }

        public Chat GetChat(int id)
        {
            return _context.Chats
                .Include(x => x.Messages)
                .FirstOrDefault(x => x.ChatId == id);
        }

        public IEnumerable<Chat> GetChats(string userId)
        {
            return _context.Chats
                .Include(x => x.Users)
                .Where(x => !x.Users
                    .Any(y => y.UserId == userId))
                .ToList();
        }

        public IEnumerable<Chat> GetPrivateChats(string userId)
        {
            return _context.Chats
                   .Include(x => x.Users)
                       .ThenInclude(x => x.User)
                   .Where(x => x.Type == ChatType.Private
                       && x.Users
                           .Any(y => y.UserId == userId))
                   .ToList();
        }

        public async Task JoinChat(int chatId, string userId)
        {
            var chatUser = new ChatUser
            {
                ChatId = chatId,
                UserId = userId,
                Role = UserRole.Member
            };

            _context.ChatUsers.Add(chatUser);

            await _context.SaveChangesAsync();
        }
    }
}