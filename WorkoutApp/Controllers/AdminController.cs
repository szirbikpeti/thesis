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

namespace WorkoutApp.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/admin")]
  public class AdminController: ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IFeedbackRepository _feedback;
    private readonly IAdminRepository _admin;

    public AdminController(
      IMapper mapper,
      UserManager<UserEntity> userManager,
      IAdminRepository admin,
      IFeedbackRepository feedback)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _admin = admin ?? throw new ArgumentNullException(nameof(admin));
      _feedback = feedback ?? throw new ArgumentNullException(nameof(feedback));
    }

    [HttpGet("users")]
    [Authorize(Policies.ManageUsers)]
    public async Task<ActionResult<ICollection<GetUserDto>>> ListUsersAsync(CancellationToken cancellationToken)
    {
      var fetchedUsers = await _admin.ListAsync(cancellationToken);

      var userListDto = fetchedUsers.Select(_ => {
        var userDto = _mapper.Map<GetUserDto>(_);

        userDto.Roles = _.Roles
          .Select(__ => __.Role.Name)
          .ToImmutableList();

        userDto.Permissions = _.Roles
          .SelectMany(__ => __.Role.Claims)
          .Where(__ => __.ClaimType == Claims.Type)
          .Select(__ => __.ClaimValue)
          .ToImmutableList();

        return userDto;
      });

      return Ok(userListDto);
    }
    
    [HttpGet("feedbacks")]
    [Authorize(Policies.ManageFeedbacks)]
    public async Task<ActionResult<ICollection<GetFeedbackDto>>> ListAsync(CancellationToken cancellationToken)
    {
      var feedbacks = await _feedback
        .DoListAsync(cancellationToken)
        .ConfigureAwait(false);

      var feedbackListDto = feedbacks.Select(_ => _mapper.Map<GetFeedbackDto>(_));

      return Ok(feedbackListDto);
    }

    [HttpDelete("{id}")]
    [Authorize(Policies.ManageUsers)]
    public async Task<IActionResult> BlockUserAsync(
      [FromRoute] [Required] int id,
      CancellationToken cancellationToken)
    {
      var user = await _userManager.FindByIdAsync(id.ToString())
        .ConfigureAwait(false);

      if (!user.LockoutEnabled) {
        return BadRequest("User cannot be blocked");
      }

      var result = await _admin.DoBlockUserAsync(user, cancellationToken)
        .ConfigureAwait(false);

      if (!result) {
        return Problem("Cannot block user");
      }
      
      return NoContent();
    }

    [HttpPost("{id}")]
    [Authorize(Policies.ManageUsers)]
    public async Task<IActionResult> RestoreUserAsync(
      [FromRoute] [Required] int id,
      CancellationToken cancellationToken)
    {
      var user = await _userManager.FindByIdAsync(id.ToString())
        .ConfigureAwait(false);

      if (user.LockoutEnd is null) {
        return BadRequest("User is not blocked");
      }

      var result = await _admin.DoRestoreUserAsync(user, cancellationToken)
        .ConfigureAwait(false);

      if (!result) {
        return Problem("Cannot restore user");
      }
      
      return NoContent();
    }
  }
}