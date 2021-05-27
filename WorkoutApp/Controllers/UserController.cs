using System;
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

    public UserController(IMapper mapper,  UserManager<UserEntity> userManager, IUserRepository user)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _user = user ?? throw new ArgumentNullException(nameof(user));
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
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var user = await _user.UpdateAsync(currentUserId, updateUserDto, cancellationToken)
        .ConfigureAwait(false);

      if (user is null) {
        return NotFound();
      }

      var userDto = _mapper.Map<GetUserDto>(user);
  
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