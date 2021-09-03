using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkoutApp.Abstractions;
using WorkoutApp.Dto;
using WorkoutApp.Entities;
using WorkoutApp.Extensions;

namespace WorkoutApp.Controllers
{
  [Authorize(Policies.SendMessages)]
  [ApiController]
  [Route("api/message")]
  public class MessageController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IMessageRepository _message;
    private readonly INotificationRepository _notification;

    public MessageController(
      IMapper mapper,
      UserManager<UserEntity> userManager,
      IMessageRepository message,
      INotificationRepository notification)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _message = message ?? throw new ArgumentNullException(nameof(message));
      _notification = notification ?? throw new ArgumentNullException(nameof(notification));
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<GetUserDto>>> ListUsersWithMessageAsync(
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);

      var users = await _message.DoListUsersWithMessageAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);

      var userListDto = users.Select(_ => _mapper.Map<GetUserDto>(_));

      return Ok(userListDto);
    }
    
    [HttpGet("{triggeredUserId}")]
    public async Task<ActionResult<ICollection<GetMessageDto>>> ListAsync(
      [FromRoute] [Required] int triggeredUserId,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var messages = await _message
        .DoListAsync(currentUserId, triggeredUserId, cancellationToken)
        .ConfigureAwait(false);

      var messageListDto = messages.Select(_ => _mapper.Map<GetMessageDto>(_));

      return Ok(messageListDto);
    }
    
    [HttpPost]
    public async Task<ActionResult<GetMessageDto>> AddAsync(
      [FromBody] [Required] MessageAdditionDto newMessage,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var mappedMessage = _mapper.Map<MessageEntity>(newMessage);
      mappedMessage.SenderUserId = currentUserId;
      mappedMessage.SentOn = DateTimeOffset.Now;

      var createdMessage = await _message
        .DoAddAsync(mappedMessage, cancellationToken)
        .ConfigureAwait(false);

      if (createdMessage is null) {
        return Problem("Message cannot be sent");
      }
      
      var notification = new NotificationEntity {
        Type = NotificationType.AddMessage,
        SentByUserId = currentUserId,
        ReceivedUserId = mappedMessage.TriggeredUserId,
        TriggeredOn = DateTimeOffset.Now
      };

      await _notification
        .DoDeleteAsync(notification.SentByUserId, notification.ReceivedUserId, notification.Type, cancellationToken)
        .ConfigureAwait(false);
      
      await _notification.DoAddAsync(notification, cancellationToken)
        .ConfigureAwait(false);
      
      await _notification.DoBroadcastNotifications(notification.ReceivedUserId)
        .ConfigureAwait(false);

      await _message.DoBroadcastMessage(notification.ReceivedUserId)
        .ConfigureAwait(false);

      var messageDto = _mapper.Map<GetMessageDto>(createdMessage);
      
      return Ok(messageDto);
    }
  }
}