using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using WorkoutApp.Abstractions;
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
    private readonly ILogger<AuthController> _logger;

    public AuthController(
      IAuthRepository auth, 
      IMapper mapper, 
      IConfiguration configuration,
      UserManager<UserEntity> userManager,
      SignInManager<UserEntity> signInManager,
      ILogger<AuthController> logger)
    {
      _auth = auth ?? throw new ArgumentNullException(nameof(auth));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("signup")]
    public async Task<ActionResult<GetUserDto>> SignUpAsync(
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
      
      await _auth.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

      var createdUser = await _userManager
        .FindByIdWithAdditionalDataAsync(mappedUser.Id, cancellationToken)
        .ConfigureAwait(false);

      var userDto = _mapper.Map<GetUserDto>(createdUser);
      
      _logger.Log(LogLevel.Information, $"Signed up with name: {newUser.UserName}");
      return Ok(userDto);
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
        return Unauthorized("User was not found or password was invalid.");
      }

      var user = await _userManager.FindByNameAsync(accessUser.UserName)
        .ConfigureAwait(false);
      
      user.LastSignedInOn = DateTimeOffset.Now;

      await _auth.SaveChangesAsync(cancellationToken);

      HttpContext.User = await _signInManager.CreateUserPrincipalAsync(user)
        .ConfigureAwait(false);

      var userDto = _mapper.Map<GetUserDto>(user);

      _logger.Log(LogLevel.Information, $"Signed in with name: {accessUser.UserName}");
      return Ok(userDto);
    }

    [Authorize]
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