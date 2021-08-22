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
using Microsoft.Extensions.Logging;

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
    private readonly IMapper _mapper;
    private readonly IFileRepository _file;
    private readonly UserManager<UserEntity> _userManager;
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly WorkoutDbContext _dbContext;
    private readonly RoleManager<RoleEntity> _roleManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
      IMapper mapper,
      IFileRepository file,
      UserManager<UserEntity> userManager,
      SignInManager<UserEntity> signInManager,
      WorkoutDbContext dbContext,
      RoleManager<RoleEntity> roleManager,
      IEmailSender emailSender,
      ILogger<AuthController> logger)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _file = file ?? throw new ArgumentNullException(nameof(file));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
      _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUpAsync(
      [FromBody] [Required] UserAdditionDto newUser,
      CancellationToken cancellationToken)
    {
      _logger.Log(LogLevel.Information, $"Starting sign up with name: {newUser.UserName}");
      
      var result = await _userManager.IsUserExistsAsync(newUser.UserName, cancellationToken);

      if (result) {
        _logger.Log(LogLevel.Information, $"Username: ({newUser.UserName}) already exists");
        ModelState.AddModelError("Username", "Username already exists");
        return BadRequest(ModelState);
      }

      var now = DateTimeOffset.Now;
      
      var mappedUser = _mapper.Map<UserEntity>(newUser);
      mappedUser.About = string.Empty;
      mappedUser.CreatedOn = now;
      mappedUser.ModifiedOn = now;

      await _userManager.CreateAsync(mappedUser).ConfigureAwait(false);

      await _userManager.AddPasswordAsync(mappedUser, newUser.Password)
        .ConfigureAwait(false);

      await _userManager.AddToRolesAsync(mappedUser, new List<string> {
        Roles.User,
      });

      var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(mappedUser)
        .ConfigureAwait(false);
      emailToken = System.Web.HttpUtility.UrlEncode(emailToken);
      var confirmationLink = $"{Request.Scheme}://{Request.Host.Value}/email-confirmation?userId={mappedUser.Id}&token={emailToken}";

      _emailSender.SendEmail(
        mappedUser.Email, "Confirm your email", 
        $"<h3>Hi, {mappedUser.UserName}!</h3> <br> Please confirm your account by <a href={confirmationLink}>clicking here</a>.");
      

      await _dbContext.SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      _logger.Log(LogLevel.Information, $"Signed up with name: {newUser.UserName}");
      
      return Ok();
    }

    [HttpGet("resend-email/{userName}")]
    public async Task<IActionResult> ResendConfirmationEmailAsync(
      [FromRoute] [Required] string userName)
    {
      var mappedUser = await _userManager.FindByNameAsync(userName)
        .ConfigureAwait(false);

      var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(mappedUser)
        .ConfigureAwait(false);
      emailConfirmationToken = System.Web.HttpUtility.UrlEncode(emailConfirmationToken);
      
      var confirmationLink = $"{Request.Scheme}://{Request.Host.Value}/email-confirmation?userId={mappedUser.Id}&token={emailConfirmationToken}";

      _emailSender.SendEmail(
        mappedUser.Email, "Confirm your email",
        $"<h3>Hi, {mappedUser.UserName} again!</h3> <br> Please confirm your account by <a href={confirmationLink}>clicking here</a>.");

      return NoContent();
    }

    [HttpPost("email-confirmation")]
    public async Task<IActionResult> ConfirmEmailAsync(EmailConfirmationDto email)
    {
      var user = await _userManager.FindByIdAsync(email.UserId)
        .ConfigureAwait(false);

      if (user is null) {
        return NotFound("User was not found.");
      }

      if (user.EmailConfirmed) {
        return BadRequest("Email is already confirmed.");
      }

      var result = await _userManager.ConfirmEmailAsync(user, email.Token)
        .ConfigureAwait(false);

      if (!result.Succeeded) {
        return Problem("Email cannot be confirmed.");
      }

      return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPasswordAsync(
      [FromBody] [Required] ForgotPasswordDto dto)
    {
      var user = await _userManager.FindByNameAsync(dto.UserName)
        .ConfigureAwait(false);

      if (user is null) {
        return NotFound("User does not found");
      }

      var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user)
        .ConfigureAwait(false);
      passwordResetToken = System.Web.HttpUtility.UrlEncode(passwordResetToken);
      
      var passwordResetLink = $"{Request.Scheme}://{Request.Host.Value}/password-reset?userId={user.Id}&token={passwordResetToken}";

      _emailSender.SendEmail(
        user.Email, "Reset your password",
        $"<h3>Hi, {user.UserName}!</h3> <br> You can reset your password by <a href={passwordResetLink}>clicking here</a>.");

      return NoContent();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(
      [FromBody] [Required] ResetPasswordDto dto)
    {
      var user = await _userManager.FindByIdAsync(dto.UserId)
        .ConfigureAwait(false);

      if (user is null) {
        return NotFound("User does not found");
      }

      var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword)
        .ConfigureAwait(false);

      if (!result.Succeeded) {
        return Problem("Password cannot be reset.");
      }

      return NoContent();
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignInAsync(
      [FromBody] [Required] AccessUserDto accessUser,
      CancellationToken cancellationToken)
    {
      _logger.Log(LogLevel.Information, $"Starting sign in with name: {accessUser.UserName}");
      
      var isExists = await _userManager.IsUserExistsAsync(accessUser.UserName, cancellationToken);

      if (!isExists) {
        _logger.Log(LogLevel.Information, "User was not found or password was invalid.");
        return BadRequest("User was not found or password was invalid.");
      }

      var result = await _signInManager.PasswordSignInAsync(accessUser.UserName, accessUser.Password, 
        isPersistent: false, lockoutOnFailure: true);

      if (!result.Succeeded) {
        var notSignedInUser = await _userManager.FindByNameAsync(accessUser.UserName)
          .ConfigureAwait(false);

        await _userManager.AccessFailedAsync(notSignedInUser)
          .ConfigureAwait(false);

        if (!notSignedInUser.EmailConfirmed) {
          _logger.Log(LogLevel.Information, "Email is not confirmed.");
          return Unauthorized("Email is not confirmed.");
        }

        if (notSignedInUser.LockoutEnd is not null) {
          var lockoutEndInMinutes = (notSignedInUser.LockoutEnd - DateTimeOffset.Now).Value.Minutes + 1;
          
          _logger.Log(LogLevel.Information, "Account is locked out.");
          return StatusCode(403, $"Account is locked out until {lockoutEndInMinutes} minutes. ({lockoutEndInMinutes})");
        }
        
        _logger.Log(LogLevel.Information, "User was not found or password was invalid.");
        return BadRequest("User was not found or password was invalid.");
      }

      var user = await _userManager
        .FindByNameWithAdditionalDataAsync(accessUser.UserName, cancellationToken)
        .ConfigureAwait(false);
      
      user!.LastSignedInOn = DateTimeOffset.Now;
      user!.LockoutEnd = null;

      await _dbContext.SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      HttpContext.User = await _signInManager.CreateUserPrincipalAsync(user)
        .ConfigureAwait(false);
      
      user!.ProfilePicture = await _file.DoGetAsync(user.ProfilePictureId, cancellationToken)
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

      await _dbContext.SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
      return Ok();
    }
  }
}