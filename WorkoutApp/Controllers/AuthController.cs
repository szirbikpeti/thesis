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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WorkoutApp.Abstractions;
using WorkoutApp.Dto;
using WorkoutApp.Entities;

namespace WorkoutApp.Controllers
{
  [ApiController]
  [Route("api/auth")]
  public class AuthController : ControllerBase
  {
    private readonly IAuthRepository _auth;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
      IAuthRepository auth, 
      IMapper mapper, 
      IConfiguration configuration,
      ILogger<AuthController> logger)
    {
      _auth = auth ?? throw new ArgumentNullException(nameof(auth));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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
        ModelState.AddModelError("Username", "Username already exists");
        return BadRequest(ModelState);
      }
      
      var mappedUser = _mapper.Map<UserEntity>(newUser);
      var createdUser = await _auth.SignUpAsync(mappedUser, newUser.Password, cancellationToken)
        .ConfigureAwait(false);

      var userDto = _mapper.Map<GetUserDto>(createdUser);

      return Ok(userDto);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignInAsync(
      [FromBody] [Required] AccessUserDto accessUser,
      CancellationToken cancellationToken)
    {
      _logger.Log(LogLevel.Information, $"Starting sign in with name: {accessUser.UserName}");
      var user = await _auth.SignInAsync(
        accessUser.UserName,
        accessUser.Password,
        cancellationToken);

      if (user is null) {
        return Unauthorized("User was not found or password was invalid");
      }

      await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(
          new ClaimsIdentity
            (new List<Claim> {new Claim(ClaimTypes.Name, accessUser.UserName)}, 
            CookieAuthenticationDefaults.AuthenticationScheme)
          )).ConfigureAwait(false);
      
      // TODO - HttpContext.User (?)

      var userDto = _mapper.Map<GetUserDto>(user);

      _logger.Log(LogLevel.Information, $"Signed in with name: {accessUser.UserName}");
      return Ok(userDto);
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> SignOutAsync()
    {
      _logger.Log(LogLevel.Information, "Starting sign out");
      await HttpContext.SignOutAsync(
          CookieAuthenticationDefaults.AuthenticationScheme)
        .ConfigureAwait(false);

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