using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkoutApp.Abstractions;
using WorkoutApp.Dto;
using WorkoutApp.Entities;
using WorkoutApp.Extensions;

namespace WorkoutApp.Controllers
{
  [Microsoft.AspNetCore.Authorization.Authorize]
  [ApiController]
  [Route("api/notification")]
  public class NotificationController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly UserManager<UserEntity> _userManager;
    private readonly INotificationRepository _notification;
    private readonly IFileRepository _file;

    public NotificationController(
      IMapper mapper,
      UserManager<UserEntity> userManager,
      INotificationRepository notification,
      IFileRepository file)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _notification = notification ?? throw new ArgumentNullException(nameof(notification));
      _file = file ?? throw new ArgumentNullException(nameof(file));
    }
    
    [HttpGet]
    public async Task<ActionResult<ICollection<GetNotificationDto>>> ListAsync(CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);

      var notifications = await _notification
        .DoListAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);

      foreach (var notification in notifications) {
        notification.SentByUser.ProfilePicture = await _file
          .DoGetAsync(notification.SentByUser.ProfilePictureId, cancellationToken)
          .ConfigureAwait(false);
      }

      var notificationListDto = notifications.Select(_ => _mapper.Map<GetNotificationDto>(_));

      return Ok(notificationListDto);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(
      [FromRoute] [Required] int id,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      await _notification.DoDeleteAsync(id, cancellationToken)
        .ConfigureAwait(false);

      await _notification.DoBroadcastFollowNotifications(currentUserId)
        .ConfigureAwait(false);
      
      return Ok();
    }
  }
}