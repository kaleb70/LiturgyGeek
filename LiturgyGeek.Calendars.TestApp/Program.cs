using LiturgyGeek.Calendars.Engine;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
            using (var program = new Program())
                Environment.Exit(program.Run(args));
        }

        public Program()
        {
            calendarReader = new CalendarReader();
            dbContext = new Data.DesignTimeContextFactory().CreateDbContext(new string[0]);
            calendarManager = new CalendarManager(dbContext);
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
