using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkoutApp.Abstractions;
using WorkoutApp.Data;
using WorkoutApp.Entities;
using WorkoutApp.Repositories;

namespace WorkoutApp
{
  public class Startup
  {
    private const string PostgreSqlConnectionName = "WorkoutConnection-Postgres";
    
    private const string AngularRootDirectoryName = "Angular";
    private const string AngularDistributionDirectoryName = "dist";
    private const string NpmScriptCommand = "start";

    private const int RequiredMinimumPasswordLength = 6;
    private const int SessionExpireTimeInHours = 7;
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews();
      services.AddSpaStaticFiles(configuration => {
        configuration.RootPath = Path.Combine(AngularRootDirectoryName, AngularDistributionDirectoryName);
      });

      services.AddDbContext<WorkoutDbContext>(_ => 
        _.UseNpgsql(Configuration.GetConnectionString(PostgreSqlConnectionName)));
      services.AddAutoMapper(typeof(Startup));

      services.AddIdentity<UserEntity, RoleEntity>(_ => {
          _.Password.RequireUppercase = false;
          _.Password.RequireLowercase = false;
          _.Password.RequireDigit = false;
          _.Password.RequireNonAlphanumeric = false;
          _.Password.RequiredLength = RequiredMinimumPasswordLength;
      })
        .AddEntityFrameworkStores<WorkoutDbContext>()
        .AddDefaultTokenProviders();
      
      services.AddAuthentication(
          CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie();
      
      services.ConfigureApplicationCookie(_ => {
        _.Cookie.SameSite = SameSiteMode.Strict;

        _.Events.OnRedirectToLogin = context => {
          context.Response.StatusCode = StatusCodes.Status401Unauthorized;
          return Task.CompletedTask;
        };

        _.Events.OnRedirectToAccessDenied = context => {
          context.Response.StatusCode = StatusCodes.Status401Unauthorized;
          return Task.CompletedTask;
        };
        
        _.ExpireTimeSpan = TimeSpan.FromHours(SessionExpireTimeInHours);
        _.SlidingExpiration = true;
          
        _.LoginPath = PathString.Empty;
        _.LogoutPath = PathString.Empty;
        _.AccessDeniedPath = PathString.Empty;
        _.ReturnUrlParameter = PathString.Empty;
      });
      
      services.Configure<CookiePolicyOptions>(_ => {
        _.MinimumSameSitePolicy = SameSiteMode.Strict;
      });

      services.AddAuthentication();
      services.AddAuthorization(options => {
        options.AddPolicy(Policies.ManageUsers, 
          _ => _.RequireClaim(Claims.Type, Claims.UserManagementPermission));
        
        options.AddPolicy(Policies.ListPosts, 
          _ => _.RequireClaim(Claims.Type, Claims.PostListPermission));
        
        options.AddPolicy(Policies.ManageWorkouts, 
          _ => _.RequireClaim(Claims.Type, Claims.WorkoutManagementPermission));
        
        options.AddPolicy(Policies.ManagePosts, 
          _ => _.RequireClaim(Claims.Type, Claims.PostManagementPermission));

        options.AddPolicy(Policies.AddCommentsToPost,
          _ => _.RequireClaim(Claims.Type, Claims.CommentAddPermission));
          
        options.AddPolicy(Policies.SendMessages, 
          _ => _.RequireClaim(Claims.Type, Claims.MessageSendPermission));
      });

      services.AddCors();
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddScoped<IAuthRepository, AuthRepository>();
      services.AddScoped<IAdminRepository, AdminRepository>();
      services.AddScoped<IUserRepository, UserRepository>();
    }

    public void Configure(IApplicationBuilder appBuilder, IWebHostEnvironment environment, WorkoutDbContext dbContext)
    {
      if (environment.IsDevelopment()) {
        appBuilder.UseDeveloperExceptionPage();
      } else {
        appBuilder.UseExceptionHandler("/Error");
        appBuilder.UseHsts();
      }

      appBuilder.UseCookiePolicy();

      appBuilder.UseHttpsRedirection();
      appBuilder.UseStaticFiles();

      if (!environment.IsDevelopment()) {
        appBuilder.UseSpaStaticFiles();
      }

      // appBuilder.UseMiddleware<JwtMiddleware>();

      dbContext.Database.Migrate(); // TODO

      appBuilder.UseRouting();
      
      appBuilder.UseAuthentication();
      appBuilder.UseAuthorization();

      appBuilder.UseCors(_ => _.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());

      appBuilder.UseEndpoints(endpoints => {
        endpoints.MapControllers();
      });

      appBuilder.UseSpa(spa => {
        spa.Options.SourcePath = AngularRootDirectoryName;

        if (environment.IsDevelopment()) {
          spa.UseAngularCliServer(npmScript: NpmScriptCommand);
        }
      });
    }
  }
}