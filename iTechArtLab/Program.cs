using DataAccessLayer.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;

namespace iTechArtLab
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File("Logs\\Info.log"))
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.File("Logs\\Warning.log"))
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.File("Logs\\Error.log"))
            .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo.File("Logs\\Fatal.log"))
            .CreateLogger();
            try
            {
                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    db.Database.Migrate();
                }

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
