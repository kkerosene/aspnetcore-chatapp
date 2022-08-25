using ReenbitChat.Models;

namespace ReenbitChat.Infrastructure.Respository
{
    public interface IChatRepository
    {
        Chat GetChat(int id);
        Task CreateChat(string name, string userId);
        Task JoinChat(int chatId, string userId);
        IEnumerable<Chat> GetChats(string userId);
        Task<int> CreatePrivateChat(string rootId, string targetId);
        IEnumerable<Chat> GetPrivateChats(string userId);

        Task<Message> CreateMessage(int chatId, string message, string userId);
    }
}