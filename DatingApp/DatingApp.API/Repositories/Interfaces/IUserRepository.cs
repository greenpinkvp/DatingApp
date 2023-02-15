using DatingApp.API.DTOs;
using DatingApp.API.Entities;

namespace DatingApp.API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);

        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByIdAsync(Guid id);

        Task<AppUser> GetUserByNameAsync(string username);

        Task<IEnumerable<MemberDto>> GetMembersAsync();

        Task<MemberDto> GetMemberByUsernameAsync(string username);

        Task<bool> SaveAsync();
    }
}