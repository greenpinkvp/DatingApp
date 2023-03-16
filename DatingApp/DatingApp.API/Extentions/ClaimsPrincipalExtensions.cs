using System.Security.Claims;

namespace DatingApp.API.Extentions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            return new Guid(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}