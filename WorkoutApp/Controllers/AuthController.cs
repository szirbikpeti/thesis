using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using WorkoutApp.Abstractions;
using WorkoutApp.Data;
using WorkoutApp.Dto;
using WorkoutApp.Entities;
using WorkoutApp.Extensions;

namespace WorkoutApp.Controllers
{
  [ApiController]
  [Route("api/auth")]
  public class AuthController : ControllerBase
  {
    private readonly IAuthRepository _auth;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly UserManager<UserEntity> _userManager;
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly WorkoutDbContext _dbContext;
    private readonly RoleManager<RoleEntity> _roleManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
      IAuthRepository auth, 
      IMapper mapper, 
      IConfiguration configuration,
      UserManager<UserEntity> userManager,
      SignInManager<UserEntity> signInManager,
      WorkoutDbContext dbContext, 
      RoleManager<RoleEntity> roleManager,
      ILogger<AuthController> logger)
    {
      _auth = auth ?? throw new ArgumentNullException(nameof(auth));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUpAsync(
      [FromBody] [Required] AdditionUserDto newUser,
      CancellationToken cancellationToken)
    {
      _logger.Log(LogLevel.Information, $"Starting sign up with name: {newUser.UserName}");
      
      var result = await _auth.IsUserExistsAsync(newUser.UserName, cancellationToken);

      if (result) {
        _logger.Log(LogLevel.Information, $"Username: ({newUser.UserName}) already exists");
        ModelState.AddModelError("Username", "Username already exists");
        return BadRequest(ModelState);
      }

      var mappedUser = _mapper.Map<UserEntity>(newUser);

      await _userManager.CreateAsync(mappedUser).ConfigureAwait(false);

      await _userManager.AddPasswordAsync(mappedUser, newUser.Password)
        .ConfigureAwait(false);

      await _userManager.AddToRolesAsync(mappedUser, new List<string> {
        Roles.User,
      });

      var createdUser = await _userManager
        .FindByIdWithAdditionalDataAsync(mappedUser.Id, cancellationToken)
        .ConfigureAwait(false);
      
      createdUser.CreatedOn = DateTimeOffset.Now;
      createdUser.ModifiedOn = DateTimeOffset.Now;
      
      await _auth.SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      _logger.Log(LogLevel.Information, $"Signed up with name: {newUser.UserName}");
      
      return Ok();
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignInAsync(
      [FromBody] [Required] AccessUserDto accessUser,
      CancellationToken cancellationToken)
    {
      _logger.Log(LogLevel.Information, $"Starting sign in with name: {accessUser.UserName}");

      var result = await _signInManager.PasswordSignInAsync(accessUser.UserName, accessUser.Password, 
        isPersistent: false, lockoutOnFailure: true);

      if (!result.Succeeded) {
        _logger.Log(LogLevel.Information, "User was not found or password was invalid.");
        return Unauthorized("User was not found or password was invalid.");
      }

      var user = await _userManager
        .FindByNameWithAdditionalDataAsync(accessUser.UserName, cancellationToken)
        .ConfigureAwait(false);
      
      user.LastSignedInOn = DateTimeOffset.Now;

      await _auth.SaveChangesAsync(cancellationToken);

      HttpContext.User = await _signInManager.CreateUserPrincipalAsync(user)
        .ConfigureAwait(false);

      var userDto = _mapper.Map<GetUserDto>(user);

      userDto.Roles = user.Roles
        .Select(_ => _.Role.Name)
        .ToImmutableList();

      userDto.Permissions = user.Roles
        .SelectMany(_ => _.Role.Claims)
        .Where(_ => _.ClaimType == Claims.Type)
        .Select(_ => _.ClaimValue)
        .ToImmutableList();

      _logger.Log(LogLevel.Information, $"Signed in with name: {accessUser.UserName}");
      return Ok(userDto);
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpDelete]
    public async Task<IActionResult> SignOutAsync()
    {
      _logger.Log(LogLevel.Information, "Starting sign out");

      await _signInManager.SignOutAsync()
        .ConfigureAwait(false);

      HttpContext.User = null!;
      
      // HttpContext.RegenerateAndStoreXsrfToken();

      _logger.Log(LogLevel.Information, "Signed out");
      return NoContent();
    }
    
    [HttpPut]
    public async Task<IActionResult> SetRoles(CancellationToken cancellationToken)
    {
      foreach (var (name, permissions) in Roles.GetRolePermissions()) {
        var role = new RoleEntity {
          Name = name,
        };

        await _roleManager.CreateAsync(role).ConfigureAwait(false);

        var claims = permissions.Select(_ => new RoleClaimEntity {
          RoleId = role.Id,
          ClaimType = Claims.Type,
          ClaimValue = _,
        });

        await _dbContext.RoleClaims.AddRangeAsync(claims, cancellationToken).ConfigureAwait(false);
      }

      await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
      return Ok();
    }

    private string GenerateToken(IIdentityAwareEntity user)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value);
      var tokenDescriptor = new SecurityTokenDescriptor {
        Subject = new ClaimsIdentity(new[] {new Claim("id", user.Id.ToString())}),
        Expires = DateTime.UtcNow.AddDays(1),
        SigningCredentials =
          new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
      };
      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }
  }
}