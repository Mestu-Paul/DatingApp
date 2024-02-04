namespace API.Interface
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMessageRepository MessageRepository { get; }
        ILiksRepository LiksRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}