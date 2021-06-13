using System;
using System.Collections.Immutable;
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

    [HttpGet]
    public async Task<ActionResult<GetUserDto>> GetByIdAsync(CancellationToken cancellationToken) // TODO: delete this function
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var fetchedUser = await _user.GetByIdAsync(currentUserId, cancellationToken);

      if (fetchedUser is null) {
        return NotFound();
      }

      var userDto = _mapper.Map<GetUserDto>(fetchedUser); 
  
      return Ok(userDto);
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

      var updatedUser = await _user.UpdateAsync(currentUser, updateUserDto, cancellationToken)
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