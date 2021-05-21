using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutApp.Abstractions;
using WorkoutApp.Dto;
using WorkoutApp.Entities;

namespace WorkoutApp.Controllers
{
  [Authorize(Policies.ManageWorkouts)]
  [ApiController]
  [Route("api/workout")]
  public class WorkoutController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly IWorkoutRepository _workout;

    public WorkoutController(IMapper mapper, IWorkoutRepository workout)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _workout = workout ?? throw new ArgumentNullException(nameof(workout));
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<ICollection<WorkoutDto>>> ListAsync(
      int userId,
      CancellationToken cancellationToken)
    {
      var fetchedWorkouts = await _workout.ListAsync(userId, cancellationToken)
        .ConfigureAwait(false);

      var workoutDtoList = _mapper.Map<WorkoutDto>(fetchedWorkouts);

      return Ok(workoutDtoList);
    }

    [HttpPost("add/{userId}")]
    public async Task<IActionResult> AddAsync(
      int userId,
      [FromBody] [Required] WorkoutDto newWorkoutDto,
      CancellationToken cancellationToken)
    {
      var mappedWorkout = _mapper.Map<WorkoutEntity>(newWorkoutDto);
      mappedWorkout.UserId = userId;

      await _workout.DoAddAsync(mappedWorkout, cancellationToken)
        .ConfigureAwait(false);

      return Ok();
    }
  }
}