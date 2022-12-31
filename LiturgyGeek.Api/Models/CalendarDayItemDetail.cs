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
            Class = churchRuleResult.RuleGroup.Key + "_" + churchRuleResult.Rule.Key;
        }

        public CalendarDayItemDetail(ChurchEvent churchEvent)
        {
            Title = churchEvent.LongName ?? churchEvent.Name ?? churchEvent.OccasionKey ?? "[missing name]"; ;
            Class = (churchEvent.EventRankKey ?? "")
                    + " "
                    + (churchEvent.OccasionKey ?? "")
                    + " "
                    + string.Join(' ', churchEvent.CustomFlags);
        }
    }
}
