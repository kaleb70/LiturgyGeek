using LiturgyGeek.Calendars.Engine;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace LiturgyGeek.Calendars.TestApp
{
    internal class Program : IDisposable
    {
        private Data.LiturgyGeekContext dbContext;
        private CalendarReader calendarReader;
        private CalendarManager calendarManager;

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
                services.AddScoped<CalendarManager>();
                services.AddScoped<Program>();
            });

            var host = builder.Build();

            using (var scope = host.Services.CreateScope())
                scope.ServiceProvider.GetRequiredService<Program>().Run(args);
        }

        public Program(Data.LiturgyGeekContext dbContext, CalendarReader calendarReader, CalendarManager calendarManager)
        {
            this.dbContext = dbContext;
            this.calendarReader = calendarReader;
            this.calendarManager = calendarManager;
        }

        int Run(string[] args)
        {
            switch (args.FirstOrDefault())
            {
                case "-internal" when args.Length == 3:
                    Internal(args[1], DateTime.Parse(args[2]));
                    return 0;

                default:
                    Console.WriteLine("Usage:");
                    Console.WriteLine();
                    Console.WriteLine("  TestApp -internal {calendarCode} {date}");
                    return args.Length == 0 ? 0 : 1;
            }
        }

        void Internal(string calendarCode, DateTime date)
        {
            var calendarItems = calendarManager.GetCalendarItems(calendarCode, date).ToArray();

            Console.WriteLine(JsonSerializer.Serialize(calendarItems, new JsonSerializerOptions()
            {
                WriteIndented = true,
            }));
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}
