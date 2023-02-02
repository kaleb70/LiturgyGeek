using LiturgyGeek.Calendars.Engine;
using LiturgyGeek.Calendars.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Tools.UploadOccasions
{
    public class UploadOccasions
    {
        private readonly CalendarReader calendarReader = new CalendarReader();

        public void Run(string[] args)
        {
            var occasionsFilename = "occasions.txt";
            if (args.Length > 0)
            {
                occasionsFilename = args[0];
                args = args.Skip(1).ToArray();
            }

            var connectionString = @"Server=dawn-treader-sql;Database=liturgygeek3_dev;User Id=sa;Password=devpw;TrustServerCertificate=True;";
            if (args.Length >= 3 && args[1] == "-connection")
                connectionString = args[2];

            Data.Occasion[] occasions;

            using (var lineStream = new FileStream(occasionsFilename, FileMode.Open))
                occasions = calendarReader.ReadOccasions(lineStream);

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(connectionString);
            using (var context = new Data.LiturgyGeekContext(optionsBuilder.Options))
            {
                var existingOccasions = context.Occasions.ToArray();
                var newOccasions = occasions.Where(no => !existingOccasions.Any(eo => eo.OccasionCode == no.OccasionCode))
                                    .ToArray();
                var modifiedOccasions = occasions.Where(no =>
                                            existingOccasions.Any(eo => eo.OccasionCode == no.OccasionCode
                                                                        && eo.DefaultName != no.DefaultName))
                                        .ToArray();

                Console.WriteLine();

                if (newOccasions.Length > 10)
                    Console.WriteLine($"Adding {newOccasions.Length} occasions");
                else if (newOccasions.Length > 0)
                {
                    Console.WriteLine("Adding new occasions:");
                    foreach (var occasion in newOccasions)
                        Console.WriteLine($"    {occasion.OccasionCode}");
                }

                Console.WriteLine();

                if (modifiedOccasions.Length > 10)
                    Console.WriteLine($"Modifying {modifiedOccasions.Length} occasions");
                else if (modifiedOccasions.Length > 0)
                {
                    Console.WriteLine("Modifying occasions:");
                    foreach (var occasion in modifiedOccasions)
                        Console.WriteLine($"    {occasion.OccasionCode}");
                }

                if (newOccasions.Length == 0 && modifiedOccasions.Length == 0)
                    Console.WriteLine("All occasions are up to date");

                else
                {
                    Console.Write("Confirm (Y/N): ");
                    if (Console.ReadLine()!.ToLower() != "y")
                        return;

                    context.Occasions.AddRange(newOccasions);

                    foreach (var newOccasion in modifiedOccasions)
                    {
                        var existingOccasion = existingOccasions.Single(o => o.OccasionCode == newOccasion.OccasionCode);
                        existingOccasion.DefaultName = newOccasion.DefaultName;
                    }

                    context.SaveChanges();

                    Console.WriteLine();
                    Console.WriteLine("Done.");
                    Console.WriteLine();
                }
            }
        }
    }
}
