using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
