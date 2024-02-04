using API.DataTransferObjects;
using API.Entities;
using API.Helpers;
using API.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectedId)
        {
            return await _context.Connections.FindAsync(connectedId);
        }

        public async Task<Group> GetGroupForConnection(string connectedId)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectedId == connectedId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(x => x.MessageSent)
                .AsQueryable();
            
            query = messageParams.Container switch {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.UserName && u.RecipientDeleted==false),
                "Outbox" =>  query.Where(u => u.SenderUserName == messageParams.UserName && u.SenderDeleted==false),
                _ => query.Where(u => u.RecipientName == messageParams.UserName && u.RecipientDeleted==false && u.DateRead==null )
            };

            var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDTO>
                .CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessagesThread(string currentUserName, string recepientName)
        {
            var query = _context.Messages
                .Where(
                    m => m.RecipientName == currentUserName && m.RecipientDeleted==false
                    && m.SenderUserName == recepientName
                    || m.RecipientName == recepientName && m.SenderDeleted==false
                    && m.SenderUserName == currentUserName
                )
                .OrderBy(m => m.MessageSent)
                .AsQueryable();

            var unreadMessages = query.Where(m => m.DateRead==null
                && m.RecipientName == currentUserName).ToList();
            
            if(unreadMessages.Any()){
                foreach(var message in unreadMessages){
                    message.DateRead = DateTime.UtcNow;
                }
            }
            return await query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

    }
}