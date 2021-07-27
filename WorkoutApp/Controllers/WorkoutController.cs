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
    public async Task<ActionResult<ICollection<GetWorkoutDto>>> ListAsync(
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var fetchedWorkouts = await _workout.ListAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);

      var workoutDtoList = fetchedWorkouts
        .Select(workout => {
          var mappedWorkoutDto = _mapper.Map<GetWorkoutDto>(workout);
          mappedWorkoutDto.RelatedPost = _mapper.Map<GetPostDto>(workout.Post);
          
          mappedWorkoutDto.Files = workout.FileRelationEntities
            .Select(relation => _mapper.Map<GetFileDto>(relation.File))
            .ToImmutableList();
          
          return mappedWorkoutDto;
        });

      return Ok(workoutDtoList);
    }
    

    [HttpGet("unposted")]
    public async Task<ActionResult<ICollection<GetWorkoutDto>>> ListUnPostedWorkoutsAsync(
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var fetchedWorkouts = await _workout.ListUnPostedAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);

      var workoutDtoList = fetchedWorkouts
        .Select(workout => {
          var mappedWorkoutDto = _mapper.Map<GetWorkoutDto>(workout);
          mappedWorkoutDto.RelatedPost = _mapper.Map<GetPostDto>(workout.Post);
          
          mappedWorkoutDto.Files = workout.FileRelationEntities
            .Select(relation => _mapper.Map<GetFileDto>(relation.File))
            .ToImmutableList();
          
          return mappedWorkoutDto;
        });

      return Ok(workoutDtoList);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetWorkoutDto>> GetAsync(
      int id,
      CancellationToken cancellationToken)
    {
      var fetchedWorkout = await _workout.DoGetAsync(id, cancellationToken)
        .ConfigureAwait(false);

      if (fetchedWorkout is null) {
        return BadRequest($"There is no workout with this id ({id})");
      }

      var workoutDto = _mapper.Map<GetWorkoutDto>(fetchedWorkout);
      workoutDto.RelatedPost = _mapper.Map<GetPostDto>(fetchedWorkout.Post);

      workoutDto.Files = fetchedWorkout.FileRelationEntities
        .Select(relation => _mapper.Map<GetFileDto>(relation.File))
        .ToImmutableList();
      
      return Ok(workoutDto);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(
      [FromBody] [Required] WorkoutAdditionDto newWorkoutDto,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      var now = DateTimeOffset.Now;
      
      var mappedWorkout = _mapper.Map<WorkoutEntity>(newWorkoutDto);
      mappedWorkout.UserId = currentUserId;
      mappedWorkout.CreatedOn = now;
      mappedWorkout.ModifiedOn = now;

      await _workout.DoAddAsync(mappedWorkout, newWorkoutDto.FileIds, cancellationToken)
        .ConfigureAwait(false);

      return Ok();
    }

    [HttpPatch("{workoutId}")]
    public async Task<ActionResult<WorkoutModificationDto>> UpdateAsync(
      [FromRoute] [Required] int workoutId,
      [FromBody] [Required] WorkoutModificationDto updatedWorkoutDto,
      CancellationToken cancellationToken)
    {
      var workout = await _workout.DoUpdateAsync(workoutId, updatedWorkoutDto, cancellationToken)
        .ConfigureAwait(false);

      if (workout is null) {
        return NotFound();
      }

      var workoutDto = _mapper.Map<GetWorkoutDto>(workout);
  
      return Ok(workoutDto);
    }

    [HttpDelete("{workoutId}")]
    public async Task<IActionResult> DeleteAsync(
      [FromRoute] [Required] int workoutId,
      CancellationToken cancellationToken)
    {
      await _workout.DoDeleteAsync(workoutId, cancellationToken)
        .ConfigureAwait(false);

      return Ok();
    }
  }
}