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
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkoutApp.Abstractions;
using WorkoutApp.Data;
using WorkoutApp.Entities;
using WorkoutApp.Frameworks;
using WorkoutApp.Hubs;
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
    private const int MaxFailedAccessAttemptsBeforeLockout = 7;
    private const int SessionExpireTimeInHours = 7;
    private const string AllowedUserNameCharacters = "AÁBCDEÉFGHIÍJKLMNOÓÖŐPQRSTUÚÜŰVWXYZaábcdeéfghiíjklmnoóöőpqrstuúüűvwxyz0123456789";
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

      services.AddDbContext<WorkoutDbContext>(_ => {
        _.UseNpgsql(Configuration.GetConnectionString(PostgreSqlConnectionName));
        
        _.ConfigureWarnings(builder => {
          builder.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning);
          builder.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning);
        });
      });
      services.AddAutoMapper(typeof(Startup));

      services.AddIdentity<UserEntity, RoleEntity>(_ => {
          _.Password.RequireUppercase = false;
          _.Password.RequireLowercase = false;
          _.Password.RequireDigit = false;
          _.Password.RequireNonAlphanumeric = false;
          _.Password.RequiredLength = RequiredMinimumPasswordLength;

          _.User.RequireUniqueEmail = true;
          _.User.AllowedUserNameCharacters = AllowedUserNameCharacters;
          _.User.RequireUniqueEmail = false;
          _.SignIn.RequireConfirmedEmail = true;
          
          _.Lockout.MaxFailedAccessAttempts = MaxFailedAccessAttemptsBeforeLockout;
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
        
        options.AddPolicy(Policies.ManageFeedbacks, 
          _ => _.RequireClaim(Claims.Type, Claims.FeedbackManagementPermission));

        options.AddPolicy(Policies.ManageWorkouts, 
          _ => _.RequireClaim(Claims.Type,
            Claims.WorkoutListPermission,
            Claims.WorkoutAddPermission,
            Claims.WorkoutUpdatePermission,
            Claims.WorkoutDeletePermission));
        
        options.AddPolicy(Policies.ManagePosts, 
          _ => _.RequireClaim(Claims.Type, 
            Claims.PostListPermission,
            Claims.PostAddPermission,
            Claims.PostUpdatePermission,
            Claims.PostDeletePermission));

        options.AddPolicy(Policies.ManageComments,
          _ => _.RequireClaim(Claims.Type, 
            Claims.CommentAddPermission,
            Claims.CommentUpdatePermission,
            Claims.CommentDeletePermission));
          
        options.AddPolicy(Policies.SendMessages, 
          _ => _.RequireClaim(Claims.Type, Claims.MessageSendPermission));
      });

      services.AddSignalR();
      services.AddCors();
      services.AddHttpContextAccessor();
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      
      services.AddScoped<IAdminRepository, AdminRepository>();
      services.AddScoped<IUserRepository, UserRepository>();
      services.AddScoped<IFileRepository, FileRepository>();
      services.AddScoped<IWorkoutRepository, WorkoutRepository>();
      services.AddScoped<INotificationRepository, NotificationRepository>();
      services.AddScoped<IPostRepository, PostRepository>();
      services.AddScoped<IFeedbackRepository, FeedbackRepository>();
      services.AddScoped<IMessageRepository, MessageRepository>();
      services.AddScoped<IEmailSender, EmailSender>();
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

      dbContext.Database.Migrate(); // TODO

      appBuilder.UseRouting();
      
      appBuilder.UseAuthentication();
      appBuilder.UseAuthorization();

      appBuilder.UseCors(_ => _.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());

      appBuilder.UseEndpoints(endpoints => {
        endpoints.MapControllers();
        endpoints.MapHub<HubClient>("/notify");
        endpoints.MapHub<HubClient>("/notify-message");
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