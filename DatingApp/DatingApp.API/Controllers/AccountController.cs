using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")] //api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExist(registerDto.UserName))
            {
                return BadRequest("UserName is taken");
            }

            //using: giải phóng dung lượng bộ nhớ của biến
            //không cần biến này sau khi sử dụng nó
            //using var hmac = new HMACSHA512();

            var user = _mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.UserName.ToLower();
            //user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            //user.PasswordSalt = hmac.Key;

            //_context.Users.Add(user);
            //await _context.SaveChangesAsync();

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }

            return new UserDto()
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender,
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var userExist = await _userManager.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

            if (userExist == null)
            {
                return Unauthorized("UserName is not exist.");
            }

            //using var hmac = new HMACSHA512(userExist.PasswordSalt);

            //var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userLogin.Password));

            //for (int i = 0; i < computedHash.Length; i++)
            //{
            //    if (computedHash[i] != userExist.PasswordHash[i])
            //    {
            //        return Unauthorized("Invalid password");
            //    }
            //}

            var result = await _userManager.CheckPasswordAsync(userExist, loginDto.Password);

            if (!result)
            {
                return Unauthorized("Invalid password");
            }

            return new UserDto()
            {
                UserName = userExist.UserName,
                Token = await _tokenService.CreateToken(userExist),
                PhotoUrl = userExist.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = userExist.KnownAs,
                Gender = userExist.Gender,
            };
        }

        private async Task<bool> UserExist(string userName)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == userName.ToLower());
        }
    }
}