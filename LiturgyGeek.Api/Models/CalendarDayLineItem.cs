namespace LiturgyGeek.Api.Models
{
    public class CalendarDayLineItem
    {
        public string Summary { get; set; }

        public string Elaboration { get; set; }

        public string[] Classes { get; set; }

        public CalendarDayLineItem(string summary, string elaboration, params string[] classes)
        {
            Summary = summary;
            Elaboration = elaboration;
            Classes = classes;
        }
    }
}
