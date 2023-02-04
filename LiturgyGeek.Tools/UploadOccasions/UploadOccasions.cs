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
        private readonly CalendarReader calendarReader;
        private readonly Data.LiturgyGeekContext dbContext;

        public UploadOccasions(CalendarReader calendarReader, Data.LiturgyGeekContext dbContext)
        {
            this.calendarReader = calendarReader;
            this.dbContext = dbContext;
        }

        public void Run(string[] args)
        {
            var occasionsFilename = "occasions.txt";
            if (args.Length > 0 && !args[0].StartsWith('/') && !args[0].StartsWith("--"))
            {
                occasionsFilename = args[0];
                args = args.Skip(1).ToArray();
            }

            Data.Occasion[] occasions;

            using (var lineStream = new FileStream(occasionsFilename, FileMode.Open))
                occasions = calendarReader.ReadOccasions(lineStream);

            var existingOccasions = dbContext.Occasions.ToArray();
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

                dbContext.Occasions.AddRange(newOccasions);

                foreach (var newOccasion in modifiedOccasions)
                {
                    var existingOccasion = existingOccasions.Single(o => o.OccasionCode == newOccasion.OccasionCode);
                    existingOccasion.DefaultName = newOccasion.DefaultName;
                }

                dbContext.SaveChanges();

                Console.WriteLine();
                Console.WriteLine("Done.");
                Console.WriteLine();
            }
        }
    }
}
