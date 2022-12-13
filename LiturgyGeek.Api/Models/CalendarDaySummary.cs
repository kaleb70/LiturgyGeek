﻿using System.ComponentModel.DataAnnotations;

namespace LiturgyGeek.Api.Models
{
    public class CalendarDaySummary
    {
        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Day { get; set; }

        [Required]
        public string MonthName { get; set; }

        public CalendarDaySummary(int year, int month, int day, string monthName)
        {
            Year = year;
            Month = month;
            Day = day;
            MonthName = monthName;
        }
    }
}
