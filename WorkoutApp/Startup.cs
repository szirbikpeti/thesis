using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkoutApp.Abstractions;
using WorkoutApp.Data;
using WorkoutApp.Repositories;

namespace WorkoutApp
{
  public class Startup
  {
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews();
      services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });

      services.AddDbContext<WorkoutDbContext>(_ => 
        _.UseNpgsql(Configuration.GetConnectionString("WorkoutConnection")));
      services.AddAutoMapper(typeof(Startup));

      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options => {
          options.Cookie.SameSite = SameSiteMode.Strict;
          
          options.ExpireTimeSpan = TimeSpan.FromHours(12);
          options.SlidingExpiration = true;
          
          options.LoginPath = PathString.Empty;
          options.LogoutPath = PathString.Empty;
          options.AccessDeniedPath = PathString.Empty;
          options.ReturnUrlParameter = PathString.Empty;
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
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller}/{action=Index}/{id?}");
      });

      appBuilder.UseSpa(spa => {
        spa.Options.SourcePath = "ClientApp";

        if (environment.IsDevelopment()) {
          spa.UseAngularCliServer(npmScript: "start");
        }
      });
    }
  }
}