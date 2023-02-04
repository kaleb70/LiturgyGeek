using LiturgyGeek.Calendars.Engine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LiturgyGeek.Tools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureHostConfiguration(hostConfig =>
            {
                hostConfig.AddEnvironmentVariables("LiturgyGeek_");
            });
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<Data.LiturgyGeekContext>(optionsBuilder =>
                {
                    Data.DesignTimeContextFactory.Configure(context, optionsBuilder);
                });

                services.AddSingleton<CalendarReader>();
                services.AddScoped<UploadCalendar.UploadCalendar>();
                services.AddScoped<UploadOccasions.UploadOccasions>();
            });

            var host = builder.Build();

            args[0] = args[0].ToLower();

            switch (args[0])
            {
                case "uc":
                case "uploadcalendar":
                    using (var scope = host.Services.CreateScope())
                        scope.ServiceProvider.GetRequiredService<UploadCalendar.UploadCalendar>().Run(args.Skip(1).ToArray());
                    break;

                case "uo":
                case "uploadoccasions":
                    using (var scope = host.Services.CreateScope())
                        scope.ServiceProvider.GetRequiredService<UploadOccasions.UploadOccasions>().Run(args.Skip(1).ToArray());
                    break;
            }
        }
    }
}
