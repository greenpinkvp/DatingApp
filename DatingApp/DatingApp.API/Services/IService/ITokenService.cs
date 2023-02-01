using DatingApp.API.Entities;

namespace DatingApp.API.Services.IService
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}