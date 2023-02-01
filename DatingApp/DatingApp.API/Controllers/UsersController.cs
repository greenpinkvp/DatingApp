using DatingApp.API.Data;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    //[Authorize]
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AppUser>> GetAllUsers()
        {
            var users = _context.Users.ToList();
            return users;
        }

        [HttpGet("{id:guid}")]
        public ActionResult<AppUser> GetUser(Guid id)
        {
            var user = _context.Users.Find(id);

            if(user == null)
            {
                return NotFound();
            }

            return user;
        }
    }
}
