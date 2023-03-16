using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Extentions;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using DatingApp.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepos;
        private readonly IMessageRepository _messageRepos;
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepos, IMessageRepository messageRepos, IMapper mapper)
        {
            _userRepos = userRepos;
            _messageRepos = messageRepos;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var userName = User.GetUserName();
            if (userName == createMessageDto.RecipientUserName.ToLower())
            {
                return BadRequest("You can not send messages to yourself");
            }

            var sender = await _userRepos.GetUserByUserNameAsync(userName);
            var recipient = await _userRepos.GetUserByUserNameAsync(createMessageDto.RecipientUserName);

            if (recipient == null)
            {
                return NotFound();
            }

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepos.AddMessage(message);

            if (await _messageRepos.SaveAllAsync())
            {
                return Ok(_mapper.Map<MessageDto>(message));
            }

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.UserName = User.GetUserName();
            var messages = await _messageRepos.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }

        [HttpGet("thread/{userName}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string userName)
        {
            var currentUserName = User.GetUserName();

            return Ok(await _messageRepos.GetMessageThread(currentUserName, userName));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(Guid id)
        {
            var userName = User.GetUserName();
            var message = await _messageRepos.GetMessage(id);

            if (message.SenderUserName != userName && message.RecipientUserName != userName)
            {
                return Unauthorized();
            }

            if (message.SenderUserName == userName)
            {
                message.SenderDeleted = true;
            }

            if (message.RecipientUserName == userName)
            {
                message.RecipientDeleted = true;
            }

            if(message.SenderDeleted && message.RecipientDeleted)
            {
                _messageRepos.DeleteMessage(message);
            }

            if(await _messageRepos.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("Problem deleting this message");
        }

    }
}