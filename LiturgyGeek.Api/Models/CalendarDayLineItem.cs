using LiturgyGeek.Framework.Calendars;

namespace LiturgyGeek.Api.Models
{
    public class CalendarDayLineItem
    {
        public string Summary { get; set; }

        public string? Elaboration { get; set; }

        public string Class { get; set; }

        public CalendarDayLineItem(string summary, string elaboration, string @class)
        {
            Summary = summary;
            Elaboration = elaboration;
            Class = @class;
        }

        public CalendarDayLineItem(ChurchRuleResult churchRuleResult)
        {
            Summary = churchRuleResult.Rule.Value.Summary;
            Elaboration = churchRuleResult.Rule.Value.Elaboration;
            Class = churchRuleResult.RuleGroup.Key + "_" + churchRuleResult.Rule.Key;
        }

        public CalendarDayLineItem(ChurchEvent churchEvent)
        {
            Summary = churchEvent.Name ?? churchEvent.LongName ?? churchEvent.OccasionKey ?? "[missing name]"; ;
            Elaboration = churchEvent.LongName;
            Class = (churchEvent.EventRankKey ?? "")
                    + " "
                    + (churchEvent.OccasionKey ?? "")
                    + " "
                    + string.Join(' ', churchEvent.CustomFlags);
        }
    }
}
