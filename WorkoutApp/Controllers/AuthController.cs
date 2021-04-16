using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

    public AuthController(IAuthRepository auth, IMapper mapper, IConfiguration configuration)
    {
      _auth = auth ?? throw new ArgumentNullException(nameof(auth));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    [HttpPost("signup")]
    public async Task<ActionResult<GetUserDto>> SignUpAsync(
      [FromBody] [Required] AdditionUserDto newUser,
      CancellationToken cancellationToken)
    {
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
      var user = await _auth.SignInAsync(
        accessUser.UserName,
        accessUser.Password,
        cancellationToken);

      if (user is null) {
        return Unauthorized();
      }

      var token = GenerateToken(user);
      var userDto = _mapper.Map<GetUserDto>(user);

      return Ok(new {token, userDto});
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