using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Extentions;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using DatingApp.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepo;
        private readonly ILikesRepository _likeRepo;

        public LikesController(IUserRepository userRepo, ILikesRepository likeRepo)
        {
            _userRepo = userRepo;
            _likeRepo = likeRepo;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceId = User.GetUserId();
            var likeUser = await _userRepo.GetUserByUsernameAsync(username);
            var sourceUser = await _likeRepo.GetUserWithLikes(sourceId);

            if (likeUser == null)
            {
                return NotFound();
            }

            if (sourceUser.Username == username)
            {
                return BadRequest("You cannot like yourself");
            }

            var userLike = await _likeRepo.GetUserLike(sourceId, likeUser.Id);

            if (userLike != null)
            {
                return BadRequest("You already like this user");
            }

            userLike = new UserLike
            {
                SourceUserId = sourceId,
                TargetUserId = likeUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await _userRepo.SaveAsync())
            {
                return Ok();
            }

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikeParams likeParams)
        {
            likeParams.UserId = User.GetUserId();

            var users = await _likeRepo.GetUserLikes(likeParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, 
                users.TotalCount, users.TotalPages));

            return Ok(users);
        }
    }
}