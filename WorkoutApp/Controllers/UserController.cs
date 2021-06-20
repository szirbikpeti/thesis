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

    public UserController(IMapper mapper,  UserManager<UserEntity> userManager, IUserRepository user, IFileRepository file)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _user = user ?? throw new ArgumentNullException(nameof(user));
      _file = file ?? throw new ArgumentNullException(nameof(file));
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

      var userDtoList = foundedUsers
        .Select(user => _mapper.Map<GetUserDto>(user));

      return Ok(userDtoList);
    }

    [HttpPut]
    public async Task<ActionResult<GetUserDto>> UpdateAsync(
      UpdateUserDto updateUserDto,
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

      var userDto = _mapper.Map<GetUserDto>(updatedUser);
      
      userDto.Roles = updatedUser.Roles
        .Select(_ => _.Role.Name)
        .ToImmutableList();

      userDto.Permissions = updatedUser.Roles
        .SelectMany(_ => _.Role.Claims)
        .Where(_ => _.ClaimType == Claims.Type)
        .Select(_ => _.ClaimValue)
        .ToImmutableList();
  
      return Ok(userDto);
    }

    [HttpPost("{id}")]
    public async Task<ActionResult<GetUserDto>> FollowUserAsync(
      [FromRoute] [Required] int id,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);

      await _user.DoFollowUserAsync(currentUserId, id, cancellationToken)
        .ConfigureAwait(false);

      var newlyFetchedCurrentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);

      var userDto = _mapper.Map<GetUserDto>(newlyFetchedCurrentUser);
      
      userDto.RequestingUserIds = newlyFetchedCurrentUser!.RequestingUsers.Select(_ => _.LeftId).ToImmutableList();
      userDto.RequestedUserIds = newlyFetchedCurrentUser!.RequestedUsers.Select(_ => _.RightId).ToImmutableList();

      return Ok(userDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<GetUserDto>> UnFollowUserAsync(
      [FromRoute] [Required] int id,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);

      await _user.DoUnFollowUserAsync(currentUserId, id, cancellationToken)
        .ConfigureAwait(false);

      var newlyFetchedCurrentUser = await _userManager
        .FindByIdWithAdditionalDataAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);

      var userDto = _mapper.Map<GetUserDto>(newlyFetchedCurrentUser);
      
      userDto.RequestingUserIds = newlyFetchedCurrentUser!.RequestingUsers.Select(_ => _.LeftId).ToImmutableList();
      userDto.RequestedUserIds = newlyFetchedCurrentUser!.RequestedUsers.Select(_ => _.RightId).ToImmutableList();

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
  }
}