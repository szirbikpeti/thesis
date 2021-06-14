using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
  [Authorize(Policies.ManageWorkouts)]
  [ApiController]
  [Route("api/workout")]
  public class WorkoutController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IWorkoutRepository _workout;

    public WorkoutController(IMapper mapper, UserManager<UserEntity> userManager, IWorkoutRepository workout)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _workout = workout ?? throw new ArgumentNullException(nameof(workout));
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<WorkoutDto>>> ListAsync(
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var fetchedWorkouts = await _workout.ListAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);

      var workoutDtoList = _mapper.Map<WorkoutDto>(fetchedWorkouts);

      return Ok(workoutDtoList);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(
      [FromBody] [Required] WorkoutDto newWorkoutDto,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var mappedWorkout = _mapper.Map<WorkoutEntity>(newWorkoutDto);
      mappedWorkout.UserId = currentUserId;
      mappedWorkout.CreatedOn = DateTimeOffset.Now;
      mappedWorkout.ModifiedOn = DateTimeOffset.Now;

      await _workout.DoAddAsync(mappedWorkout, newWorkoutDto.FileIds, cancellationToken)
        .ConfigureAwait(false);

      return Ok();
    }

    [HttpPatch("{workoutId}")]
    public async Task<ActionResult<WorkoutDto>> UpdateAsync(
      [FromRoute] [Required] int workoutId,
      [FromBody] [Required] WorkoutDto updatedWorkoutDto,
      CancellationToken cancellationToken)
    {
      var workout = await _workout.DoUpdateAsync(workoutId, updatedWorkoutDto, cancellationToken)
        .ConfigureAwait(false);

      if (workout is null) {
        return NotFound();
      }

      var workoutDto = _mapper.Map<WorkoutDto>(workout);
  
      return Ok(workoutDto);
    }
  }
}