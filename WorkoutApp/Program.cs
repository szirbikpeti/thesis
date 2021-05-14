using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace WorkoutApp
{
  public class Program
  {
    private const string LogFilesDirectoryName = "Logger/logs";
      
    private const string LogOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} " +
                                                 "[{Level:u1}] {SourceContext}: {Message:lj}{NewLine}{Exception}";
                 
    public static void Main(string[] args) =>
      CreateHostBuilder(args).Build().Run();


    private static IHostBuilder CreateHostBuilder(string[] args)
    {
      return Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
        .UseSerilog((hostBuilderContext, loggerConfiguration) => {
          loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
          
          loggerConfiguration.Enrich.FromLogContext();

          loggerConfiguration.WriteTo.Console(
            outputTemplate: LogOutputTemplate
            );

          loggerConfiguration.WriteTo.File(
            path: Path.Combine(LogFilesDirectoryName, ".log"),
            outputTemplate: LogOutputTemplate,
            formatProvider: CultureInfo.InvariantCulture,
            buffered: true,
            shared: false,
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true,
            retainedFileCountLimit: 60,
            encoding: Encoding.UTF8
            );
        });
    }
  }
}