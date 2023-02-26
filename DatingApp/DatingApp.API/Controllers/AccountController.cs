using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")] //api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExist(registerDto.Username))
            {
                return BadRequest("Username is taken");
            }

            //using: giải phóng dung lượng bộ nhớ của biến
            //không cần biến này sau khi sử dụng nó
            using var hmac = new HMACSHA512();

            var user = _mapper.Map<AppUser>(registerDto);

            user.Username = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto()
            {
                Username = user.Username,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender,
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto userLogin)
        {
            var userExist = await _context.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Username == userLogin.Username.ToLower());

            if (userExist == null)
            {
                return Unauthorized("Username is not exist.");
            }

            using var hmac = new HMACSHA512(userExist.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userLogin.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != userExist.PasswordHash[i])
                {
                    return Unauthorized("Invalid password");
                }
            }

            return new UserDto()
            {
                Username = userExist.Username,
                Token = _tokenService.CreateToken(userExist),
                PhotoUrl = userExist.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = userExist.KnownAs,
                Gender = userExist.Gender,
            };
        }

        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username == username.ToLower());
        }
    }
}