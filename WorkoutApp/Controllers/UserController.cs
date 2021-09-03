using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using WorkoutApp.Frameworks;

namespace WorkoutApp.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/user")]
  public class UserController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IUserRepository _user;
    private readonly INotificationRepository _notification;

    public UserController(
      IMapper mapper,
      UserManager<UserEntity> userManager,
      IUserRepository user,
      INotificationRepository notification)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _user = user ?? throw new ArgumentNullException(nameof(user));
      _notification = notification ?? throw new ArgumentNullException(nameof(notification));
    }

    [Authorize(Policies.SendMessages)]
    [HttpGet("{id}")]
    public async Task<ActionResult<GetUserDto>> GetByIdAsync(
      [FromRoute] [Required] int id,
      CancellationToken cancellationToken)
    {
      var foundedUser = await _userManager
        .FindByIdWithAdditionalDataAsync(id, includesRoles: false, includesFollowsData:false, includesProfilePicture: true, cancellationToken)
        .ConfigureAwait(false);

      if (foundedUser is null) {
        return NotFound("User was not found");
      }

      var userDto = _mapper.Map<GetUserDto>(foundedUser);
      
      return Ok(userDto);
    }

    [HttpGet("search/{name}")]
    public async Task<ActionResult<ICollection<GetUserDto>>> ListByNameAsync(
      [FromRoute] [Required] string name,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var foundedUsers = await _userManager
        .ListByUserAndFullNameAsync(currentUserId, name, cancellationToken)
        .ConfigureAwait(false);

      var userDtoList = foundedUsers
        .Select(user => _mapper.Map<GetUserDto>(user));

      return Ok(userDtoList);
    }

    [HttpGet("friends")]
    public async Task<ActionResult<ICollection<GetFriendsDto>>> ListFriendsAsync(
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var followerUsers = await _userManager
        .ListFollowerUsersAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);
      
      var followedUsers = await _userManager
        .ListFollowedUsersAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);

      var userEqualityComparer = new IdEqualityComparer<UserEntity>();

      followerUsers = followerUsers.Except(followedUsers, userEqualityComparer).ToImmutableList();
      
      var followerListDto = followerUsers
        .Select(user => _mapper.Map<GetUserDto>(user)).ToImmutableList();

      var followedListDto = followedUsers
        .Select(user => _mapper.Map<GetUserDto>(user)).ToImmutableList();

      var friendDto = new GetFriendsDto {
        FollowerUsers = followerListDto,
        FollowedUsers = followedListDto
      };

      return Ok(friendDto);
    }

    [HttpGet("followRequestsAndFollows")]
    public async Task<ActionResult<GetFollowRequestsAndFollowsDto>> GetFollowRequestsAndFollowsAsync(
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var currentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, includesRoles: false, cancellationToken:  cancellationToken)
        .ConfigureAwait(false);

      var dto = CreateFollowRequestsAndFollowsDto(currentUser!);
      
      return Ok(dto);
    }

    [HttpPut]
    public async Task<ActionResult<GetUserDto>> UpdateAsync(
      [FromBody] [Required] UpdateUserDto updateUserDto,
      CancellationToken cancellationToken)
    {
      var currentUser = await _userManager.GetUserAsync(HttpContext.User)
        .ConfigureAwait(false);

      if (currentUser is null) {
        return NotFound();
      }

      if (updateUserDto.PasswordChange.OldPassword != string.Empty) {
        var result = await _userManager.ChangePasswordAsync(
            currentUser, 
            updateUserDto.PasswordChange.OldPassword, 
            updateUserDto.PasswordChange.NewPassword)
          .ConfigureAwait(false);

        if (!result.Succeeded) {
          return BadRequest("Old password is incorrect.");
        }
      }

      var updatedUser = await _user.DoUpdateAsync(currentUser, updateUserDto, cancellationToken)
        .ConfigureAwait(false);

      var userDto = CreateUserDto(updatedUser!);
  
      return Ok(userDto);
    }

    [HttpPost("request/{id}")]
    public async Task<ActionResult<GetFollowRequestsAndFollowsDto>> AddFollowRequestAsync(
      [FromRoute] [Required] int id,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);

      await _user.DoAddFollowRequestAsync(currentUserId, id, cancellationToken)
        .ConfigureAwait(false);

      var notification = new NotificationEntity {
        Type = NotificationType.FollowRequest,
        SentByUserId = currentUserId,
        ReceivedUserId = id,
        TriggeredOn = DateTimeOffset.Now
      };
      
      await _notification.DoAddAsync(notification, cancellationToken)
        .ConfigureAwait(false);
      
      await _notification.DoBroadcastNotifications(id)
        .ConfigureAwait(false);
      
      var newlyFetchedCurrentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, includesRoles: false, cancellationToken:  cancellationToken)
        .ConfigureAwait(false);
      
      var dto = CreateFollowRequestsAndFollowsDto(newlyFetchedCurrentUser!);

      return Ok(dto);
    }

    [HttpDelete("request/{id}")]
    public async Task<ActionResult<GetFollowRequestsAndFollowsDto>> DeleteFollowRequestAsync(
      [FromRoute] [Required] int id,
      [FromBody] [Required] bool isDeletedByTargetUser,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var sourceId = currentUserId;
      var targetId = id;

      if (isDeletedByTargetUser) {
        sourceId = id;
        targetId = currentUserId;
      }

      await _user.DoDeleteFollowRequestAsync(sourceId, targetId, cancellationToken)
        .ConfigureAwait(false);
      
      await _notification.DoDeleteAsync(
          sourceId, targetId, 
          isDeletedByTargetUser 
            ? NotificationType.FollowRequest 
            : NotificationType.DeclineFollowRequest, cancellationToken)
        .ConfigureAwait(false);
      
      var notification = new NotificationEntity {
        Type = isDeletedByTargetUser 
          ? NotificationType.DeleteFollowRequest 
          : NotificationType.DeleteDeclinedFollowRequest,
        SentByUserId = sourceId,
        ReceivedUserId = targetId,
        TriggeredOn = DateTimeOffset.Now
      };
      
      await _notification.DoAddAsync(notification, cancellationToken)
        .ConfigureAwait(false);
      
      await _notification.DoBroadcastNotifications(new []{id.ToString(), currentUserId.ToString()})
        .ConfigureAwait(false);

      var newlyFetchedCurrentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, includesRoles: false, cancellationToken:  cancellationToken)
        .ConfigureAwait(false);
      
      var dto = CreateFollowRequestsAndFollowsDto(newlyFetchedCurrentUser!);

      return Ok(dto);
    }

    [HttpPatch("request/{id}")]
    public async Task<ActionResult<GetFollowRequestsAndFollowsDto>> DeclineFollowRequest(
      [FromRoute] [Required] int id,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);

      await _user.DoDeclineFollowRequest(currentUserId, id, cancellationToken)
        .ConfigureAwait(false);

      await _notification.DoDeleteAsync(
          id, currentUserId, NotificationType.FollowRequest, cancellationToken)
        .ConfigureAwait(false);

      var notification = new NotificationEntity {
        Type = NotificationType.DeclineFollowRequest,
        SentByUserId = currentUserId,
        ReceivedUserId = id,
        TriggeredOn = DateTimeOffset.Now
      };
      
      await _notification.DoAddAsync(notification, cancellationToken)
        .ConfigureAwait(false);

      await _notification.DoBroadcastNotifications(new []{id.ToString(), currentUserId.ToString()})
        .ConfigureAwait(false);
      
      var newlyFetchedCurrentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, includesRoles: false, cancellationToken:  cancellationToken)
        .ConfigureAwait(false);
      
      var dto = CreateFollowRequestsAndFollowsDto(newlyFetchedCurrentUser!);

      return Ok(dto);
    }

    [HttpPatch("follow/{id}")]
    public async Task<ActionResult<GetFollowRequestsAndFollowsDto>> AcceptFollowRequestAsync(
      [FromRoute] [Required] int id, 
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);

      await _user.DoAcceptFollowRequestAsync(currentUserId, id, cancellationToken)
        .ConfigureAwait(false);

      await _notification.DoDeleteAsync(
id, currentUserId, NotificationType.FollowRequest, cancellationToken)
        .ConfigureAwait(false);
      
      var notification = new NotificationEntity {
        Type = NotificationType.AcceptFollowRequest,
        SentByUserId = currentUserId,
        ReceivedUserId = id,
        TriggeredOn = DateTimeOffset.Now
      };
      
      await _notification.DoAddAsync(notification, cancellationToken)
        .ConfigureAwait(false);
      
      await _notification.DoBroadcastNotifications(new []{id.ToString(), currentUserId.ToString()})
        .ConfigureAwait(false);
      
      var newlyFetchedCurrentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, includesRoles: false, cancellationToken:  cancellationToken)
        .ConfigureAwait(false);
      
      var dto = CreateFollowRequestsAndFollowsDto(newlyFetchedCurrentUser!);

      return Ok(dto);
    }

    [HttpPost("follow/{id}")]
    public async Task<ActionResult<GetFollowRequestsAndFollowsDto>> FollowBackAsync(
      [FromRoute] [Required] int id, 
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);

      await _user.DoFollowBackAsync(currentUserId, id, cancellationToken)
        .ConfigureAwait(false);
      
      var notification = new NotificationEntity {
        Type = NotificationType.FollowBack,
        SentByUserId = currentUserId,
        ReceivedUserId = id,
        TriggeredOn = DateTimeOffset.Now
      };
      
      await _notification.DoAddAsync(notification, cancellationToken)
        .ConfigureAwait(false);
      
      await _notification.DoBroadcastNotifications(id)
        .ConfigureAwait(false);
      
      var newlyFetchedCurrentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, includesRoles: false, cancellationToken:  cancellationToken)
        .ConfigureAwait(false);
      
      var dto = CreateFollowRequestsAndFollowsDto(newlyFetchedCurrentUser!);

      return Ok(dto);
    }

    [HttpDelete("follow/{id}")]
    public async Task<ActionResult<GetFollowRequestsAndFollowsDto>> UnFollowAsync(
      [FromRoute] [Required] int id,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);

      await _user.DoUnFollowAsync(currentUserId, id, cancellationToken)
        .ConfigureAwait(false);

      var newlyFetchedCurrentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, includesRoles: false, cancellationToken:  cancellationToken)
        .ConfigureAwait(false);
      
      var dto = CreateFollowRequestsAndFollowsDto(newlyFetchedCurrentUser!);

      return Ok(dto);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var result = await _user.DoDeleteAsync(currentUserId, cancellationToken);

      if (!result) {
        return NotFound();
      }

      return Ok();
    }

    private GetUserDto CreateUserDto(UserEntity user)
    {
      if (user is null) {
        throw new ArgumentNullException(nameof(user));
      }
      
      var userDto = _mapper.Map<GetUserDto>(user);

      userDto.Roles = user!.Roles
        .Select(_ => _.Role.Name)
        .ToImmutableList();

      userDto.Permissions = user!.Roles
        .SelectMany(_ => _.Role.Claims)
        .Where(_ => _.ClaimType == Claims.Type)
        .Select(_ => _.ClaimValue)
        .ToImmutableList();

      return userDto;
    }

    private static GetFollowRequestsAndFollowsDto CreateFollowRequestsAndFollowsDto(UserEntity user)
    {
      if (user is null) {
        throw new ArgumentNullException(nameof(user));
      }
      
      return new GetFollowRequestsAndFollowsDto {
        SourceUsers = user.SourceUsers
          .Select(_ => new GetFollowRequestDto {Id = _.SourceId, IsBlocked = _.IsBlocked})
          .ToImmutableList(),
        TargetUsers = user.TargetUsers
          .Select(_ => new GetFollowRequestDto {Id = _.TargetId, IsBlocked = _.IsBlocked})
          .ToImmutableList(),
        FollowerUserIds = user.FollowerUsers.Select(_ => _.FollowerId).ToImmutableList(),
        FollowedUserIds = user.FollowedUsers.Select(_ => _.FollowedId).ToImmutableList()
      };
    }
  }
}