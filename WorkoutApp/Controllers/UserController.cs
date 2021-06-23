using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
  [Route("api/user")]
  public class UserController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IUserRepository _user;
    private readonly IFileRepository _file;
    private readonly INotificationRepository _notification;

    public UserController(
      IMapper mapper,
      UserManager<UserEntity> userManager,
      IUserRepository user,
      IFileRepository file,
      INotificationRepository notification)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _user = user ?? throw new ArgumentNullException(nameof(user));
      _file = file ?? throw new ArgumentNullException(nameof(file));
      _notification = notification ?? throw new ArgumentNullException(nameof(notification));
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<ICollection<GetUserDto>>> ListByNameAsync(
      [FromRoute] [Required] string name,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var foundedUsers = await _userManager
        .ListByUserAndFullNameAsync(currentUserId, name, cancellationToken)
        .ConfigureAwait(false);
      
      foreach (var foundedUser in foundedUsers) {
        foundedUser!.ProfilePicture = await _file.DoGetAsync(foundedUser.ProfilePictureId, cancellationToken)
          .ConfigureAwait(false);
      }

      var userDtoList = foundedUsers
        .Select(user => _mapper.Map<GetUserDto>(user));

      return Ok(userDtoList);
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
          return Unauthorized();
        }
      }

      var updatedUser = await _user.DoUpdateAsync(currentUser, updateUserDto, cancellationToken)
        .ConfigureAwait(false);

      updatedUser!.ProfilePicture = await _file.DoGetAsync(updatedUser.ProfilePictureId, cancellationToken)
        .ConfigureAwait(false);

      var userDto = CreateUserDto(updatedUser!);
  
      return Ok(userDto);
    }

    [HttpPost("request/{id}")]
    public async Task<ActionResult<GetUserDto>> AddFollowRequestAsync(
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

      var newlyFetchedCurrentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);
      
      newlyFetchedCurrentUser!.ProfilePicture = await _file
        .DoGetAsync(newlyFetchedCurrentUser.ProfilePictureId, cancellationToken)
        .ConfigureAwait(false);

      var userDto = CreateUserDto(newlyFetchedCurrentUser!);

      return Ok(userDto);
    }

    [HttpDelete("request/{id}")]
    public async Task<ActionResult<GetUserDto>> DeleteFollowRequestAsync(
      [FromRoute] [Required] int id,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);

      await _user.DoDeleteFollowRequestAsync(currentUserId, id, cancellationToken)
        .ConfigureAwait(false);

      var newlyFetchedCurrentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);
      
      newlyFetchedCurrentUser!.ProfilePicture = await _file
        .DoGetAsync(newlyFetchedCurrentUser.ProfilePictureId, cancellationToken)
        .ConfigureAwait(false);

      var userDto = CreateUserDto(newlyFetchedCurrentUser!);

      return Ok(userDto);
    }

    [HttpPatch("request/{id}")]
    public async Task<ActionResult<GetUserDto>> DeclineFollowRequest(
      [FromRoute] [Required] int id, 
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);

      await _user.DoDeclineFollowRequest(currentUserId, id, cancellationToken)
        .ConfigureAwait(false);
      
      var newlyFetchedCurrentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);
      
      newlyFetchedCurrentUser!.ProfilePicture = await _file
        .DoGetAsync(newlyFetchedCurrentUser.ProfilePictureId, cancellationToken)
        .ConfigureAwait(false);

      var userDto = CreateUserDto(newlyFetchedCurrentUser!);

      return Ok(userDto);
    }

    [HttpPatch("follow/{id}")]
    public async Task<ActionResult<GetUserDto>> AcceptFollowRequestAsync(
      [FromRoute] [Required] int id, 
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);

      await _user.DoAcceptFollowRequestAsync(currentUserId, id, cancellationToken)
        .ConfigureAwait(false);
      
      var newlyFetchedCurrentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);
      
      newlyFetchedCurrentUser!.ProfilePicture = await _file
        .DoGetAsync(newlyFetchedCurrentUser.ProfilePictureId, cancellationToken)
        .ConfigureAwait(false);

      var userDto = CreateUserDto(newlyFetchedCurrentUser!);

      return Ok(userDto);
    }

    [HttpPost("follow/{id}")]
    public async Task<ActionResult<GetUserDto>> FollowBackAsync(
      [FromRoute] [Required] int id, 
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);

      await _user.DoFollowBackAsync(currentUserId, id, cancellationToken)
        .ConfigureAwait(false);
      
      var newlyFetchedCurrentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);
      
      newlyFetchedCurrentUser!.ProfilePicture = await _file
        .DoGetAsync(newlyFetchedCurrentUser.ProfilePictureId, cancellationToken)
        .ConfigureAwait(false);

      var userDto = CreateUserDto(newlyFetchedCurrentUser!);

      return Ok(userDto);
    }

    [HttpDelete("follow/{id}")]
    public async Task<ActionResult<GetUserDto>> UnFollowAsync(
      [FromRoute] [Required] int id,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);

      await _user.DoUnFollowAsync(currentUserId, id, cancellationToken)
        .ConfigureAwait(false);

      var newlyFetchedCurrentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);
      
      newlyFetchedCurrentUser!.ProfilePicture = await _file
        .DoGetAsync(newlyFetchedCurrentUser.ProfilePictureId, cancellationToken)
        .ConfigureAwait(false);

      var userDto = CreateUserDto(newlyFetchedCurrentUser!);

      return Ok(userDto);
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

      userDto.SourceUserIds = user.SourceUsers.Select(_ => _.SourceId).ToImmutableList();
      userDto.TargetUserIds = user.TargetUsers.Select(_ => _.TargetId).ToImmutableList();
      userDto.FollowerUserIds = user.FollowerUsers.Select(_ => _.FollowerId).ToImmutableList();
      userDto.FollowedUserIds = user.FollowedUsers.Select(_ => _.FollowedId).ToImmutableList();

      return userDto;
    }
  }
}