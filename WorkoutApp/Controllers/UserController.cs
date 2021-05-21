using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WorkoutApp.Abstractions;
using WorkoutApp.Dto;

namespace WorkoutApp.Controllers
{
  [Microsoft.AspNetCore.Authorization.Authorize]
  [ApiController]
  [Route("api/user")]
  public class UserController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly IUserRepository _user;

    public UserController(IMapper mapper, IUserRepository user)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _user = user ?? throw new ArgumentNullException(nameof(user));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetUserDto>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
      var fetchedUser = await _user.GetByIdAsync(id, cancellationToken);

      if (fetchedUser is null) {
        return NotFound();
      }

      var userDto = _mapper.Map<GetUserDto>(fetchedUser); 
  
      return Ok(userDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GetUserDto>> UpdateAsync(
      int id, 
      UpdateUserDto updateUserDto, 
      CancellationToken cancellationToken)
    {
      var user = await _user.UpdateAsync(id, updateUserDto, cancellationToken)
        .ConfigureAwait(false);

      if (user is null) {
        return NotFound();
      }

      var userDto = _mapper.Map<GetUserDto>(user);
  
      return Ok(userDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
      var result = await _user.DoDeleteAsync(id, cancellationToken);

      if (!result) {
        return NotFound();
      }

      return Ok();
    }
  }
}