using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common
{
    public static class TimeSpanExtensions
    {
        public static double GetTotalWeeks(this TimeSpan value) => value.TotalDays / 7;

        public static int GetWeeks(this TimeSpan value) => value.Days / 7;
    }
}
