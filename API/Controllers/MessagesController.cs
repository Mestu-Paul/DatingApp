using API.DataTransferObjects;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public MessagesController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO){
            var username = User.GetUsername();
            if(username == createMessageDTO.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");
            
            var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);
            var recepient = await _uow.UserRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);
            
            if(recepient == null)
                return NotFound();

            var message = new Message{
                Sender = sender,
                Recipient = recepient,
                SenderUserName = sender.UserName,
                RecipientName = recepient.UserName,
                Content = createMessageDTO.Content
            };

            _uow.MessageRepository.AddMessage(message); 

            if(await _uow.Complete())return Ok(_mapper.Map<MessageDTO>(message));
            return BadRequest("Faild to send message");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams){
            messageParams.UserName = User.GetUsername();

            var messages = await _uow.MessageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));
            return messages;
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id){
            var username = User.GetUsername();
            var message = await _uow.MessageRepository.GetMessage(id);

            if (message.SenderUserName != username && message.RecipientName != username)
                return Unauthorized();
                
            if(message.SenderUserName == username) message.SenderDeleted = true;
            if(message.RecipientName == username) message.RecipientDeleted = true;
            if(message.SenderDeleted && message.RecipientDeleted){
                _uow.MessageRepository.DeleteMessage(message);
            }

            if(await _uow.Complete())return Ok();
            return BadRequest("Problem deleting message");

        }
    }

}