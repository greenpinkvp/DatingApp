using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;

namespace DatingApp.API.Repositories.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(Guid sourceUserId, Guid targetUserId);

        Task<AppUser> GetUserWithLikes(Guid userId);

        Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams);
    }
}