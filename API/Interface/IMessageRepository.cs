using API.DataTransferObjects;
using API.Entities;
using API.Helpers;
using Newtonsoft.Json.Linq;

namespace API.Interface
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDTO>> GetMessagesThread(string currentUserName, string recepientName);
        Task<bool> SaveAllAsync();
    }
}