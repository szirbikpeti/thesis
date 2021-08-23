using System;
using System.Collections;
using System.Collections.Generic;
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
  [Authorize]
  [ApiController]
  [Route("api/feedback")]
  public class FeedbackController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IFeedbackRepository _feedback;

    public FeedbackController(
      IMapper mapper,
      UserManager<UserEntity> userManager,
      IFeedbackRepository feedback)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _feedback = feedback ?? throw new ArgumentNullException(nameof(feedback));
    }
    
    [HttpPost]
    public async Task<IActionResult> AddAsync(
      [FromBody] [Required] FeedbackAdditionDto newFeedback,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var mappedFeedback = _mapper.Map<FeedbackEntity>(newFeedback);
      mappedFeedback.UserId = currentUserId;
      mappedFeedback.CreatedOn = DateTimeOffset.Now;

      await _feedback
        .DoAddAsync(mappedFeedback, cancellationToken)
        .ConfigureAwait(false);
      
      return Ok();
    }
  }
}