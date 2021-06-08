using System;
using System.Collections.Generic;
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
  [Route("api/admin")]
  public class AdminController: ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IUserRepository _user;
    private readonly IAdminRepository _admin;

    public AdminController(IMapper mapper, UserManager<UserEntity> userManager, IUserRepository user, IAdminRepository admin)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _user = user ?? throw new ArgumentNullException(nameof(user));
      _admin = admin ?? throw new ArgumentNullException(nameof(admin));
    }

    [Authorize(Policies.ManageUsers)]
    [HttpGet]
    public async Task<ActionResult<ICollection<GetUserDto>>> ListUsersAsync(CancellationToken cancellationToken)
    {
      var fetchedUsers = await _admin.ListAsync(cancellationToken);

      var users = _mapper.Map<ICollection<GetUserDto>>(fetchedUsers);

      return Ok(users);
    }
  }
}