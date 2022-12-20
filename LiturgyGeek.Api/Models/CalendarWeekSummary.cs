using System.ComponentModel.DataAnnotations;

namespace LiturgyGeek.Api.Models
{
    public class CalendarWeekSummary
    {
        [Required]
        public CalendarDaySummary[]? Days { get; set; }

        public bool HasHeadlines => Days?.Any(d => d?.Headlines?.Length > 0) ?? false;
    }
}
