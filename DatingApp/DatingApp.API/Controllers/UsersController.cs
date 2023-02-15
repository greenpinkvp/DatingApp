using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    //[Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllUsers()
        {
            var users = await _userRepo.GetMembersAsync();

            return Ok(users);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MemberDto>> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return _mapper.Map<MemberDto>(user);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepo.GetMemberByUsernameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
    }
}