using DatingApp.API.Entities;

namespace DatingApp.API.Services.IService
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}