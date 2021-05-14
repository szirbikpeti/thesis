using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutApp.Abstractions;
using WorkoutApp.Dto;

namespace WorkoutApp.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/admin")]
  public class AdminController: ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly IUserRepository _user;
    private readonly IAdminRepository _admin;

    public AdminController(IMapper mapper, IUserRepository user, IAdminRepository admin)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _user = user ?? throw new ArgumentNullException(nameof(user));
      _admin = admin ?? throw new ArgumentNullException(nameof(admin));
    }

    [Authorize(Policies.ManageUsers)]
    [HttpGet("{id}")]
    public async Task<ActionResult<ICollection<GetUserDto>>> ListUsersAsync(int id, CancellationToken cancellationToken)
    {
      var fetchedAdminUser = await _user.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

      if (fetchedAdminUser is null) {
        return NotFound();
      }

      var fetchedUsers = await _admin.ListAsync(cancellationToken);

      var users = _mapper.Map<ICollection<GetUserDto>>(fetchedUsers);

      return Ok(users);
    }
  }
}