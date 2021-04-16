using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WorkoutApp.Entities;

namespace WorkoutApp.Data
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public class AuthorizeAttribute : Attribute, IAuthorizationFilter
  {
    public void OnAuthorization(AuthorizationFilterContext context)
    {
      var user = (UserEntity) context.HttpContext.Items["User"];

      if (user is null) {
        context.Result = new JsonResult(new {message = "Unauthorized"})
          {StatusCode = StatusCodes.Status401Unauthorized};
      }
    }
  }
}