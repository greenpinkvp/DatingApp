using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;

namespace DatingApp.API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);

        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByIdAsync(Guid id);

        Task<AppUser> GetUserByUserNameAsync(string userName);

        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);

        Task<MemberDto> GetMemberByUserNameAsync(string userName, bool isCurrentUser);

        Task<string> GetUserGender(string userName);

        Task<AppUser> GetUserByMainPhotoId(Guid mainPhotoId);
    }
}