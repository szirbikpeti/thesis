using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Data
{
  public class JwtMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
      _next = next ?? throw new ArgumentNullException((nameof(next)));
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task Invoke(HttpContext context, IUserRepository user)
    {
      var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

      if (token is not null) {
        AttachUserToContext(context, user, token);
      }

      await _next(context);
    }
    
    private async void AttachUserToContext(HttpContext context, IUserRepository user, string token)
    {
      try
      {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value);
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false,
          // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
          ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

        context.Items["User"] = await user.GetByIdAsync(userId);
      }
      catch {
        // ignored
      }
    }
  }
}