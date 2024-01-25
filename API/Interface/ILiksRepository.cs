using API.DataTransferObjects;
using API.Entities;
using API.Helpers;

namespace API.Interface
{
    public interface ILiksRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams);
    }
}