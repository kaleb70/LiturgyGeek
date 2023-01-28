﻿using LiturgyGeek.Calendars.Engine;
using LiturgyGeek.Calendars.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiturgyGeek.Tools.UploadCalendar
{
    public class UploadCalendar
    {
        private readonly CalendarReader calendarReader = new CalendarReader();

        public void Run(string[] args)
        {
            var jsonFilename = $"{args[0]}.json";
            var lineFilename = $"{args[0]}.txt";
            ChurchCalendar churchCalendar;
            using (var jsonStream = new FileStream(jsonFilename, FileMode.Open))
            {
                using (var lineStream = File.Exists(lineFilename)
                                        ? new FileStream(lineFilename, FileMode.Open)
                                        : null)
                {
                    churchCalendar = calendarReader.Read(jsonStream, lineStream);
                }
            }

            using (var context = new Data.DesignTimeContextFactory().CreateDbContext(new string[0]))
            {
                var allOccasions = context.Occasions.ToArray();

                var undefinedOccasions = churchCalendar.GetAllOccasionCodes()
                                            .Where(c => !allOccasions.Any(o => o.OccasionCode == c));

                if (undefinedOccasions.Any())
                {
                    Console.WriteLine();
                    Console.WriteLine($"Undefined occasions found in calendar {args[0]}");
                    foreach (var occasionCode in undefinedOccasions)
                        Console.WriteLine($"    {occasionCode}");
                }

                var existingCalendar = context.Calendars
                                        .Where(c => c.CalendarCode == churchCalendar.CalendarCode)
                                        .Include(c => c.CalendarDefinition)
                                        .SingleOrDefault();

                var jsonForDb = JsonSerializer.Serialize(churchCalendar);

                if (existingCalendar?.CalendarDefinition?.Definition == jsonForDb)
                    Console.WriteLine($"Calendar {churchCalendar.CalendarCode} is already in the database.");

                else
                {
                    if (existingCalendar != null)
                    {
                        Console.WriteLine($"Existing calendar {churchCalendar.CalendarCode} will be replaced.");
                        Console.Write("Confirm (Y/N): ");
                        if (Console.ReadLine()!.ToLower() != "y")
                            return;

                        context.Calendars.Remove(existingCalendar);
                    }

                    var dataCalendar = new Data.Calendar()
                    {
                        CalendarCode = churchCalendar.CalendarCode,
                        CalendarDefinition = new()
                        {
                            Definition = JsonSerializer.Serialize(churchCalendar),
                        },

                        ChurchRules = churchCalendar.RuleGroups
                                        .SelectMany(g => g.Value.Rules,
                                                    (g, r) => new Data.ChurchRule()
                                                    {
                                                        RuleGroupCode = g.Key,
                                                        RuleCode = r.Key,
                                                        Summary = r.Value.Summary,
                                                        Elaboration = r.Value.Elaboration,
                                                    })
                                        .ToArray(),
                    };

                    context.Calendars.Add(dataCalendar);

                    context.SaveChanges();
                }
            }
        }
    }
}