using LiturgyGeek.Framework.Calendars;
using Microsoft.OpenApi.Services;

namespace LiturgyGeek.Api.Models
{
    public class CalendarDaySummaryItem
    {
        public string Summary { get; set; }

        public string? Elaboration { get; set; }

        public string Class { get; set; }

        public CalendarDaySummaryItem(string summary, string elaboration, string @class)
        {
            Summary = summary;
            Elaboration = elaboration;
            Class = @class;
        }

        public CalendarDaySummaryItem(ChurchRuleResult churchRuleResult)
        {
            Summary = churchRuleResult.Rule.Value.Summary;
            Elaboration = churchRuleResult.Rule.Value.Elaboration;
            Class = $"rule_{churchRuleResult.RuleGroup.Key} rule_{churchRuleResult.RuleGroup.Key}_{churchRuleResult.Rule.Key}";
        }

        public CalendarDaySummaryItem(ChurchEventResult churchEventResult)
        {
            Summary = churchEventResult.Event.Name ?? churchEventResult.Event.LongName
                        ?? churchEventResult.Event.OccasionKey ?? "[missing name]";
            Class = (churchEventResult.Event.EventRankKey ?? "")
                    + " "
                    + (churchEventResult.Event.OccasionKey ?? "")
                    + " "
                    + (churchEventResult.TransferredFrom.HasValue ? "transferred" : "")
                    + " "
                    + string.Join(' ', churchEventResult.Event.CustomFlags);
        }
    }
}
