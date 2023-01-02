using LiturgyGeek.Framework.Calendars;

namespace LiturgyGeek.Api.Models
{
    public class CalendarDayItemDetail
    {
        public string Title { get; set; }

        public string? Elaboration { get; set; }

        public string Class { get; set; }

        public CalendarDayItemDetail(string title, string elaboration, string @class)
        {
            Title = title;
            Elaboration = elaboration;
            Class = @class;
        }

        public CalendarDayItemDetail(ChurchRuleResult churchRuleResult)
        {
            Title = churchRuleResult.Rule.Value.Summary;
            Elaboration = churchRuleResult.Rule.Value.Elaboration;
            Class = $"rule_{churchRuleResult.RuleGroup.Key} rule_{churchRuleResult.RuleGroup.Key}_{churchRuleResult.Rule.Key}";
        }

        public CalendarDayItemDetail(ChurchEventResult churchEventResult)
        {
            Title = churchEventResult.Event.LongName ?? churchEventResult.Event.Name
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
