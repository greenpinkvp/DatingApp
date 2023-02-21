using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Extentions;
using DatingApp.API.Interfaces;
using DatingApp.API.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepo, IMapper mapper, IPhotoService photoService)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _photoService = photoService;
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

        [HttpPut]
        public async Task<ActionResult> UpdateUserAsync(MemberUpdateDto memberUpdateDto)
        {
            var user = await _userRepo.GetUserByUsernameAsync(User.GetUsername());

            if (user == null)
            {
                return NotFound();
            }

            _mapper.Map(memberUpdateDto, user);

            if (await _userRepo.SaveAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to update user.");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepo.GetUserByUsernameAsync(User.GetUsername());

            if (user == null)
            {
                return NotFound();
            }

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _userRepo.SaveAsync())
            {
                //return CreatedAtAction(nameof(GetUserByUsernameAsync),
                //    new { username = user.Username }, _mapper.Map<PhotoDto>(photo));

                return _mapper.Map<PhotoDto>(photo);
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(Guid photoId)
        {
            var user = await _userRepo.GetUserByUsernameAsync(User.GetUsername());

            if (user == null)
            {
                return NotFound();
            }

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null)
            {
                return NotFound();
            }

            if (photo.IsMain)
            {
                return BadRequest("This is already your main photo");
            }

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            if (currentMain != null)
            {
                currentMain.IsMain = false;
            }

            photo.IsMain = true;

            if (await _userRepo.SaveAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem setting the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(Guid photoId)
        {
            var user = await _userRepo.GetUserByUsernameAsync(User.GetUsername());

            if (user == null)
            {
                return NotFound();
            }

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null)
            {
                return NotFound();
            }

            if (photo.IsMain)
            {
                return BadRequest("You cannot delete your main photo");
            }

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                if (result.Error != null)
                {
                    return BadRequest(result.Error.Message);
                }
            }

            user.Photos.Remove(photo);

            if (await _userRepo.SaveAsync())
            {
                return Ok();
            }

            return BadRequest("Problem deleting photo");
        }
    }
}