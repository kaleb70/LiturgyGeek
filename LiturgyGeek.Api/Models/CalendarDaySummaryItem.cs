using LiturgyGeek.Framework.Calendars;

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
            Class = churchRuleResult.RuleGroup.Key + "_" + churchRuleResult.Rule.Key;
        }

        public CalendarDaySummaryItem(ChurchEvent churchEvent)
        {
            Summary = churchEvent.Name ?? churchEvent.LongName ?? churchEvent.OccasionKey ?? "[missing name]"; ;
            Class = (churchEvent.EventRankKey ?? "")
                    + " "
                    + (churchEvent.OccasionKey ?? "")
                    + " "
                    + string.Join(' ', churchEvent.CustomFlags);
        }
    }
}
